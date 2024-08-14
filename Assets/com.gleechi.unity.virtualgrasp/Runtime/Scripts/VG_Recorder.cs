// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if VG_ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace VirtualGrasp.Scripts
{

    /** 
     * VG_Recorder provides a tool to record and replay hand interactions in an object-independent manner.
     * The MonoBehavior provides a tutorial on the VG API functions for recording and replaying interactions.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_component_vgrecorder." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_Recorder : MonoBehaviour
    {
        public enum RecordingMode
        {
            RECORD_ON_PLAY,
            REPLAY_ON_PLAY,
            MANUAL
        }

        [Tooltip("How to start recording or replaying a sequence; MANUAL means record and replay will be initiated through Keycodes specified.")]
        public RecordingMode m_recordingMode = RecordingMode.MANUAL;
        //[Tooltip("Reset scene when replay.")]
        //public bool m_resetWhenReplay = false;
        [Tooltip("Path to save the new Recording asset in. E.g. Assets/Recordings/example.sdb.")]
        public string m_newRecordingPath = "Assets/Recordings/example.sdb";
        [Tooltip("Recording(s) used for replay")]
        public List<VG_Recording> m_replayRecordings = new List<VG_Recording>();

        [Tooltip("Avatar used to replay a sensor recording. Has to be the avatar with Replay checked.")]
        public List<SkinnedMeshRenderer> m_replayAvatars = null;

        [Header("Keycodes")]
#if VG_ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        [Tooltip("Key to press to start and stop recording.")]
        public Key m_recordingKey = Key.R;
        [Tooltip("Key to press to replay a recording. Shift + Key to Pause/Resume.")]
        public Key m_replaySequenceKey = Key.P;
        [Tooltip("Key to press to replay a single recording element. Shift + Key to Pause/Resume.")]
        public Key m_replaySegmentKey = Key.Y;
#else
        [Tooltip("Key to press to start and stop recording. When stop recording most recent recording will be saved to the New Recording Path.")]
        public KeyCode m_recordingKey = KeyCode.R;
        [Tooltip("Key to press to replay a recording. Shift + Key to Pause/Resume.")]
        public KeyCode m_replaySequenceKey = KeyCode.P;
        [Tooltip("Key to press to replay a single recording element. Shift + Key to Pause/Resume.")]
        public KeyCode m_replaySegmentKey = KeyCode.Y;
#endif

        [Header("Segment Replay Options")]
        [Tooltip("Avatar's hand to replay a specific interaction segment with.")]
        public VG_HandSide m_side = VG_HandSide.LEFT;
        [Tooltip("Specific segment to replay (see console output for segment ID, and has to corresond to Side and Replay Object).")]
        public int m_segmentID = 5;
        [Tooltip("Provide object for segment replay to be centered on this object's current pose.")]
        public Transform m_replayObject = null;

        private bool m_isRecording = false;
        private bool m_isReplaying = false;
        private Dictionary<VG_HandSide, bool> m_primaryPushed = new Dictionary<VG_HandSide, bool>() { { VG_HandSide.LEFT, false }, { VG_HandSide.RIGHT, false } };
        private Dictionary<VG_HandSide, bool> m_secondaryPushed = new Dictionary<VG_HandSide, bool>() { { VG_HandSide.LEFT, false }, { VG_HandSide.RIGHT, false } };
        private Dictionary<VG_HandSide, int> m_joystickPushed = new Dictionary<VG_HandSide, int>() { { VG_HandSide.LEFT, -1 }, { VG_HandSide.RIGHT, -1 } };

        private VG_Recording m_tempRecording;
        private int m_replayID = 0;

        void Start()
        {
            VG_Controller.OnPostUpdate.AddListener(CheckStopReplay);

            switch (m_recordingMode)
            {
                case RecordingMode.RECORD_ON_PLAY:
                    TryToggleRecording();
                    break;
                case RecordingMode.REPLAY_ON_PLAY:
                    StartReplaySequence();
                    break;
            }
            if (m_newRecordingPath!=null && m_newRecordingPath.Length > 0)
                enforceAppending(ref m_newRecordingPath, ".sdb");
        }

        private void OnApplicationQuit()
        {
            if (m_isRecording) VG_Controller.StopRecording();
        }

        private void enforceAppending(ref string target, string appendingStr)
        {
            if (target.LastIndexOf(appendingStr) == -1)
            {
                Debug.LogWarning(target + " is appended with " + appendingStr);
                target = target + appendingStr;
                return;
            }

            // Make sure appendingStr is at the end of target
            string sub_str = target.Substring(target.LastIndexOf(appendingStr), target.Length - target.LastIndexOf(appendingStr));
            if(sub_str.Length != appendingStr.Length)
            {
                Debug.LogWarning(target + " is appended with " + appendingStr);
                target = target + appendingStr;
            }
        }

        void Update() {

#if VG_ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Keyboard.current[m_recordingKey].wasPressedThisFrame) TryToggleRecording();

            if (Keyboard.current[m_replaySequenceKey].wasPressedThisFrame) {
                if (Keyboard.current[Key.LeftShift].isPressed) ResumePauseReplay();
                else StartReplaySequence();
            }

            if (Keyboard.current[m_replaySegmentKey].wasPressedThisFrame) {
                if (Keyboard.current[Key.LeftShift].isPressed) ResumePauseReplay();
                else StartReplaySegment();
            }
#else
            if (Input.GetKeyDown(m_recordingKey)) TryToggleRecording();

            if (Input.GetKeyDown(m_replaySequenceKey))
            {
                if (Input.GetKey(KeyCode.LeftShift)) ResumePauseReplay();
                else StartReplaySequence();
            }

            if (Input.GetKeyDown(m_replaySegmentKey))
            {
                if (Input.GetKey(KeyCode.LeftShift)) ResumePauseReplay();
                else StartReplaySegment();
            }
#endif
        }

        public void TryToggleRecording()
        {
            if (m_isReplaying)
            {
                Debug.Log("VG is replaying. Stopping the replay before recording. Try again.");
                return;
            }

            ToggleRecording();
        }

        private void ToggleRecording()
        {
            if (!m_isRecording)
            {
               
                VG_Controller.StartRecording();
                m_isRecording = true;
                Debug.Log("Started VG sensor recording, Press Recording Key again will stop recording.");

                if (m_newRecordingPath == null || m_newRecordingPath.Length == 0)
                    Debug.LogWarning("When stop recording, data won't be save to a file because New Recording Path is not set.");
            }
            else
            {
                VG_Controller.StopRecording();
                m_isRecording = false;
                VG_Controller.CollectRecording(out m_tempRecording);
                m_replayRecordings.Add(m_tempRecording);
                Debug.Log("Stopped VG sensor recording.");

                if (m_newRecordingPath == null || m_newRecordingPath.Length == 0)
                {
                    Debug.LogWarning("Recorded sensor data can not be saved to a file because New Recording Path is not set.");
                }
                else
                {
                    VG_Controller.SaveRecording(m_newRecordingPath);
                    Debug.Log("Saved VG sensor recording as " + m_newRecordingPath);
#if UNITY_EDITOR
                    AssetDatabase.ImportAsset(m_newRecordingPath);
#endif
                }
            }
        }

        private void CheckStopReplay()
        {
            if (!m_isReplaying) return;
            foreach (int replayAvatarID in GetReplayAvatars())
            {
                if (!VG_Controller.IsReplaying(replayAvatarID))
                {
                    m_isReplaying = false;
                    m_replayID++;
                    if (m_replayID >= m_replayRecordings.Count) m_replayID = 0;
                }
            }
        }

        private bool PrepareReplay()
        {
            if (m_isRecording)
            {
                Debug.Log("VG is recording. Stop the recording before replay. Try again.");
                return false;
            }

            VG_ReturnCode res;

            res = VG_Controller.LoadRecording(m_replayRecordings[m_replayID]);
            if (res == VG_ReturnCode.SUCCESS)
            {
                m_isReplaying = true;
                return true;
            }
            else
            {
                Debug.LogError("Loading recording failed");
                m_isReplaying = false;
                return false;
            }
        }

        public void StartReplaySequence()
        {
            if (!PrepareReplay()) return;

            foreach (int replayAvatarID in GetReplayAvatars())
                VG_Controller.StartReplay(replayAvatarID);
        }

        public VG_ReturnCode StartReplaySegment()
        {
            if (m_replayObject == null)
            {
                Debug.LogWarning("You need to provide a replay object for StartSingleReplay().");
                return VG_ReturnCode.INVALID_TARGET;
            }
            if (!PrepareReplay()) return VG_ReturnCode.DLL_FUNCTION_FAILED;

            foreach (int replayAvatarID in GetReplayAvatars())
                if (VG_Controller.StartReplayOnObject(m_replayObject, replayAvatarID, m_side, m_segmentID) != VG_ReturnCode.SUCCESS)
                    return VG_ReturnCode.ARGUMENT_ERROR;

            return VG_ReturnCode.SUCCESS;
        }

        public void ResumePauseReplay()
        {
            if (m_isRecording)
            {
                Debug.Log("VG is recording. Stop the recording before replay. Try again.");
                return;
            }

            if (m_isReplaying)
            {
                foreach (int replayAvatarID in GetReplayAvatars())
                    VG_Controller.StopReplay(replayAvatarID);
            }
            else
            {
                foreach (int replayAvatarID in GetReplayAvatars())
                    VG_Controller.ResumeReplay(replayAvatarID);
            }

            m_isReplaying = !m_isReplaying;
        }

        private List<int> GetReplayAvatars()
        {
            List<int> res = new List<int>();
            int replayAvatarIDLeft, replayAvatarIDRight;
            if (m_replayAvatars == null || m_replayAvatars.Count == 0)
            {
                if (VG_Controller.GetReplayAvatarID(out replayAvatarIDLeft, out replayAvatarIDRight) != VG_ReturnCode.SUCCESS)
                    return res;
                if(replayAvatarIDLeft == replayAvatarIDRight)
                    res.Add(replayAvatarIDLeft);
                else
                {
                    if(replayAvatarIDLeft != -1)
                        res.Add(replayAvatarIDLeft);
                    if(replayAvatarIDRight != -1)
                        res.Add(replayAvatarIDRight);
                }
            }
            else
            {
                foreach (SkinnedMeshRenderer sm in m_replayAvatars)
                    res.Add(sm.GetInstanceID());
            }

            return res;
        }
    }
}