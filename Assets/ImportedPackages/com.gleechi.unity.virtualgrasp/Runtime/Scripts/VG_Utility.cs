// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using VirtualGrasp;

namespace VirtualGrasp.Scripts
{
    [CreateAssetMenu(menuName = "VirtualGrasp/VG_Utility")]
    public class VG_Utility : ScriptableObject
    {
        private Transform m_selectedTransform;
        private Dictionary<Transform, ArticulationDrive> cachedJointByTransform = new Dictionary<Transform, ArticulationDrive>();
        private static VG_HandSide BoolToHandSide(bool isLeft) => isLeft ? VG_HandSide.LEFT : VG_HandSide.RIGHT;
        private static int SensorAvatarIDLeft
        {
            get
            {
                VG_Controller.GetSensorControlledAvatarID(out var sensorAvatarIDLeft, out var sensorAvatarIDRight);
                if (sensorAvatarIDLeft != -1 || sensorAvatarIDRight != -1)
                    return sensorAvatarIDLeft;
                return ReplayAvatarIDLeft;
            }
        }
        private static int SensorAvatarIDRight
        {
            get
            {
                VG_Controller.GetSensorControlledAvatarID(out var sensorAvatarIDLeft, out var sensorAvatarIDRight);
                if (sensorAvatarIDLeft != -1 || sensorAvatarIDRight != -1)
                    return sensorAvatarIDRight;
                return ReplayAvatarIDRight;
            }
        }
        private static int ReplayAvatarIDLeft
        {
            get
            {
                VG_Controller.GetReplayAvatarID(out var replayAvatarIDLeft, out var replayAvatarIDRight);
                return replayAvatarIDLeft;
            }
        }

        private static int ReplayAvatarIDRight
        {
            get
            {
                VG_Controller.GetReplayAvatarID(out var replayAvatarIDLeft, out var replayAvatarIDRight);
                return replayAvatarIDRight;
            }
        }

        public void SelectObject(Transform transform) => m_selectedTransform = transform;

        public void SetSelectionWeight(float weight)
        {
            if (m_selectedTransform == null)
            {
                Debug.LogError("No transform select in VGUtility");
            }
            SetSelectionWeight(m_selectedTransform, weight);
        }

        private void SetSelectionWeight(Transform transform, float weight) => VG_Controller.SetObjectSelectionWeight(transform, weight);

        public void SetObjectIsDualHandsOnly(Transform transform)
        {
            VG_Controller.SetDualHandsOnly(transform, true);
        }
        public void SetObjectNotDualHandsOnly(Transform transform)
        {
            VG_Controller.SetDualHandsOnly(transform, false);
        }
        public void SetJointStateNormalized(float normalizedState)
        {
            normalizedState = Mathf.Clamp01(normalizedState);
            var articulation = FindActiveArticulation(m_selectedTransform);
            if (articulation == null)
            {
                Debug.LogError("No articulation on object, can't set joint state", m_selectedTransform);
                return;
            }
            var state = Mathf.Lerp(articulation.m_min, articulation.m_max, normalizedState);
            VG_Controller.SetObjectJointState(m_selectedTransform, state);
        }

        private static VG_Articulation FindActiveArticulation(Transform transform)
        {
            var articulations = transform.GetComponents<VG_Articulation>();
            foreach (var articulation in articulations)
            {
                if (articulation.enabled)
                {
                    return articulation;
                }
            }
            return null;
        }

        public void SetJointState(float state)
        {
            if (m_selectedTransform == null)
            {
                Debug.LogError("No transform selected, can't set joint state", this);
                return;
            }
            VG_Controller.SetObjectJointState(m_selectedTransform, state);
        }

        public void StopSettingJointState()
        {
            if (m_selectedTransform == null)
            {
                Debug.LogError("No transform selected, can't set stop set joint state", this);
                return;
            }
            VG_Controller.StopSettingObjectJointState(m_selectedTransform);
        }

        public void SetGlobalInteractionType(int type)
        {
            Debug.Log($"Set global interaction type to {(VG_InteractionType)type}");
            VG_Controller.SetGlobalInteractionType((VG_InteractionType)type);
        }

        public void SetInteractionTypeOnSelectedObject(int type)
        {
            if (this.m_selectedTransform == null)
            {
                Debug.LogError("No articulation selected, selected one with VGUtility.SelectObject first", this);
                return;
            }
            Debug.Log($"Set interaction type to {(VG_InteractionType)type}");
            VG_Controller.SetInteractionTypeForObject(this.m_selectedTransform, (VG_InteractionType)type);
        }

        public void LockArticulation(Transform transform)
        {
            if (transform.TryGetComponent<ArticulationBody>(out var articulationBody))
            {
                if (articulationBody.jointType == ArticulationJointType.FixedJoint) return;

                var xDrive = articulationBody.xDrive;
                cachedJointByTransform.Add(transform, xDrive);
                xDrive.lowerLimit = xDrive.upperLimit = articulationBody.jointPosition[0];
                articulationBody.xDrive = xDrive;
            }
            else
            {
                VG_Controller.ChangeObjectJoint(transform, VG_JointType.FIXED);
            }
        }

        public void UnLockArticulation(Transform transform)
        {
            if (transform.TryGetComponent<ArticulationBody>(out var articulationBody))
            {
                if (cachedJointByTransform.TryGetValue(transform, out var cachedJointType))
                {
                    articulationBody.xDrive = cachedJointType;
                    cachedJointByTransform.Remove(transform);
                }
                else
                {
                    Debug.LogWarning("No cached joint found for articulationBody", transform);
                }
            }
            else
            {
                if (VG_Controller.RecoverObjectJoint(transform) != VG_ReturnCode.SUCCESS)
                {
                    Debug.LogWarning("Couldn't recover articulation");
                }
            }
        }

        public void ChangeObjectJoint(VG_Articulation articulation) => VG_Controller.ChangeObjectJoint(articulation.transform, articulation);

        public void ChangeObjectJointOnSelectedObject(VG_Articulation articulation)
        {
            VG_Controller.ChangeObjectJoint(m_selectedTransform, articulation);
        }

        public void ForceReleaseObject(Transform transform) => VG_Controller.ForceReleaseObject(transform);

        public void ForceReleaseObjectLeft()
        {
            int left_id = SensorAvatarIDLeft;
            if (left_id == -1)
            {
                Debug.LogError("No avatar with left hand is registered, can not ForceReleaseObjectLeft!");
                return;
            }

            VG_Controller.ForceReleaseObject(left_id, VG_HandSide.LEFT);
        }

        public void ForceReleaseObjectRight()
        {
            int right_id = SensorAvatarIDRight;
            if (right_id == -1)
            {
                Debug.LogError("No avatar with right hand is registered, can not ForceReleaseObjectRight!");
                return;
            }

            VG_Controller.ForceReleaseObject(right_id, VG_HandSide.RIGHT);
        }

        public void ForceReleaseObject()
        {
            int left_id = SensorAvatarIDLeft;
            int right_id = SensorAvatarIDRight;
            if (left_id != -1)
                VG_Controller.ForceReleaseObject(left_id);
            if (right_id != left_id && right_id != -1)
                VG_Controller.ForceReleaseObject(right_id);
        }

        public void MakeObjectUngraspable(Transform transform)
        {
            VG_Controller.SetObjectSelectionWeight(transform, 0f);
        }

        public void MakeObjectGraspable(Transform transform)
        {
            VG_Controller.SetObjectSelectionWeight(transform, 1f);
        }
        public void JumpGraspObjectLeft(Transform transform)
        {
            int left_id = SensorAvatarIDLeft;
            if (left_id == -1)
            {
                Debug.LogError("No avatar with left hand is registered, can not JumpGraspObjectLeft!");
                return;
            }
            VG_Controller.JumpGraspObject(left_id, VG_HandSide.LEFT, transform);
        }

        public void JumpGraspObjectRight(Transform transform)
        {
            int right_id = SensorAvatarIDRight;
            if (right_id == -1)
            {
                Debug.LogError("No avatar with right hand is registered, can not JumpGraspObjectRight!");
                return;
            }
            VG_Controller.JumpGraspObject(right_id, VG_HandSide.RIGHT, transform);
        }

        public void SwitchGraspObjectLeft(Transform transform)
        {
            int left_id = SensorAvatarIDLeft;
            if (left_id == -1)
            {
                Debug.LogError("No avatar with left hand is registered, can not SwitchGraspObjectLeft!");
                return;
            }
            VG_Controller.SwitchGraspObject(left_id, VG_HandSide.LEFT, transform);
        }

        public void SwitchGraspObjectRight(Transform transform)
        {
            int right_id = SensorAvatarIDRight;
            if (right_id == -1)
            {
                Debug.LogError("No avatar with right hand is registered, can not SwitchGraspObjectRight!");
                return;
            }
            VG_Controller.SwitchGraspObject(right_id, VG_HandSide.RIGHT, transform);
        }

        public void StartReplay(string replayName)
        {
            VG_Controller.LoadRecording(replayName);
            int left_id = ReplayAvatarIDLeft;
            int right_id = ReplayAvatarIDRight;
            if (left_id != -1)
                VG_Controller.StartReplay(left_id);
            if (right_id != left_id && right_id != -1)
                VG_Controller.StartReplay(right_id);
        }

        public void StopReplay()
        {
            int left_id = ReplayAvatarIDLeft;
            int right_id = ReplayAvatarIDRight;
            if (left_id != -1)
                VG_Controller.StopReplay(left_id);
            if (right_id != left_id && right_id != -1)
                VG_Controller.StopReplay(right_id);

        }

        public void SetAvatarMirrorHand(bool mirrorHand)
        {
            int left_id = SensorAvatarIDLeft;
            int right_id = SensorAvatarIDRight;
            if (left_id != -1)
                VG_Controller.SetAvatarMirrorHandControl(left_id, mirrorHand);
            if (right_id != left_id && right_id != -1)
                VG_Controller.SetAvatarMirrorHandControl(right_id, mirrorHand);
        }
        public void SetSensorAvatarActive(bool isActive)
        {
            int left_id = SensorAvatarIDLeft;
            int right_id = SensorAvatarIDRight;
            if (left_id != -1)
                VG_Controller.SetAvatarActive(left_id, true, isActive, Vector3.down);
            if (right_id != left_id && right_id != -1)
                VG_Controller.SetAvatarActive(right_id, true, isActive, Vector3.down);
        }

        public void SetReplayAvatarActive(bool isActive)
        {
            int left_id = ReplayAvatarIDLeft;
            int right_id = ReplayAvatarIDRight;
            if (left_id != -1)
                VG_Controller.SetAvatarActive(left_id, true, isActive);
            if (right_id != left_id && right_id != -1)
                VG_Controller.SetAvatarActive(right_id, true, isActive);
        }

        public void MakeGestureLeft(int gestureID)
        {
            int left_id = SensorAvatarIDLeft;
            if (left_id == -1)
            {
                Debug.LogError("No avatar with left hand is registered, can not MakeGestureLeft!");
                return;
            }
            VG_Controller.MakeGesture(left_id, VG_HandSide.LEFT, (VG_GestureType)gestureID);
        }

        public void MakeGestureRight(int gestureID)
        {
            int right_id = SensorAvatarIDRight;
            if (right_id == -1)
            {
                Debug.LogError("No avatar with right hand is registered, can not MakeGestureRight!");
                return;
            }
            VG_Controller.MakeGesture(right_id, VG_HandSide.RIGHT, (VG_GestureType)gestureID);
        }

        public void ReleaseGestureLeft()
        {
            int left_id = SensorAvatarIDLeft;
            if (left_id == -1)
            {
                Debug.LogError("No avatar with left hand is registered, can not ReleaseGestureLeft!");
                return;
            }
            VG_Controller.ReleaseGesture(left_id, VG_HandSide.LEFT);
        }

        public void ReleaseGestureRight()
        {
            int right_id = SensorAvatarIDRight;
            if (right_id == -1)
            {
                Debug.LogError("No avatar with right hand is registered, can not ReleaseGestureRight!");
                return;
            }
            VG_Controller.ReleaseGesture(right_id, VG_HandSide.RIGHT);
        }

        public void ReleaseGesture()
        {
            int left_id = SensorAvatarIDLeft;
            int right_id = SensorAvatarIDRight;
            if (left_id != -1)
                VG_Controller.ReleaseGesture(left_id, VG_HandSide.LEFT);
            if (right_id != -1)
                VG_Controller.ReleaseGesture(right_id, VG_HandSide.RIGHT);
        }

        public void BlockReleaseObjectLeft()
        {
            int left_id = SensorAvatarIDLeft;
            if (left_id == -1)
            {
                Debug.LogError("No avatar with left hand is registered, can not BlockReleaseObjectLeft()!");
                return;
            }
            VG_Controller.SetBlockReleaseObject(left_id, VG_HandSide.LEFT, true);
        }

        public void BlockReleaseObjectRight()
        {
            int right_id = SensorAvatarIDRight;
            if (right_id == -1)
            {
                Debug.LogError("No avatar with right hand is registered, can not BlockReleaseObjectRight!");
                return;
            }
            VG_Controller.SetBlockReleaseObject(right_id, VG_HandSide.RIGHT, true);
        }

        public void BlockReleaseObject()
        {
            int left_id = SensorAvatarIDLeft;
            int right_id = SensorAvatarIDRight;
            if (left_id != -1)
                VG_Controller.SetBlockReleaseObject(left_id, VG_HandSide.LEFT, true);
            if (right_id != -1)
                VG_Controller.SetBlockReleaseObject(right_id, VG_HandSide.RIGHT, true);
        }

        public void AllowReleaseObjectLeft()
        {
            int left_id = SensorAvatarIDLeft;
            if (left_id == -1)
            {
                Debug.LogError("No avatar with left hand is registered, can not AllowReleaseObjectLeft()!");
                return;
            }
            VG_Controller.SetBlockReleaseObject(left_id, VG_HandSide.LEFT, false);
        }

        public void AllowReleaseObjectRight()
        {
            int right_id = SensorAvatarIDRight;
            if (right_id == -1)
            {
                Debug.LogError("No avatar with right hand is registered, can not AllowReleaseObjectRight!");
                return;
            }
            VG_Controller.SetBlockReleaseObject(right_id, VG_HandSide.RIGHT, false);
        }

        public void AllowReleaseObject()
        {
            int left_id = SensorAvatarIDLeft;
            int right_id = SensorAvatarIDRight;
            if (left_id != -1)
                VG_Controller.SetBlockReleaseObject(left_id, VG_HandSide.LEFT, false);
            if (right_id != -1)
                VG_Controller.SetBlockReleaseObject(right_id, VG_HandSide.RIGHT, false);
        }
    }
}