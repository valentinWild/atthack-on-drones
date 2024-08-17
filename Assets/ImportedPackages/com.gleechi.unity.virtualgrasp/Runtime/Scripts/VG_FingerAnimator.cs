// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VirtualGrasp.Scripts
{
    using static VG_FingerAnimator.VG_FingerAnimationData;

    /** 
     * VG_FingerAnimator animates the fingers on a selected hand side using a list of animation clips. 
     * Useful for making manual corrections on primary grasps,
     * or animating fingers during inhand manipulation of articulated objects.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_component_vgfingeranimator." + VG_Version.__VG_VERSION__ + ".html")]
    
    public partial class VG_FingerAnimator : MonoBehaviour
    {
        [SerializeField, Tooltip("Hand side to animate")]
        private HandSide m_hand = HandSide.LEFT;
        [SerializeField, Tooltip("Animation clips for directing finger animation")]
        private List<VG_FingerAnimationData> m_fingerAnimations = new List<VG_FingerAnimationData>();
        [SerializeField, Tooltip("Optional, only enable animation when this object is grasped. If unassigned this transform will be used.")]
        private Transform m_interactableObject = null;
        [SerializeField]
        private AnimationEvents m_events = new AnimationEvents();
        private Transform m_holdingHand = null;
        private float m_animationDrive = 1.0f;

        private Dictionary<Transform, Vector3> m_startRotation= new Dictionary<Transform, Vector3>();
        private Dictionary<Transform, Vector3> m_targetRotation = new Dictionary<Transform, Vector3>();

        /// <summary>
        /// Pass in a float in range [0,1] to drive the animation
        /// </summary>
        public void DriveAnimation(float animationDrive)
        {
            this.m_animationDrive = Mathf.Clamp01(animationDrive);
        }

        /// <summary>
        /// Stops animation drive, bones will be rotated according to values inside the animation clips
        /// </summary>
        public void StopAnimationDrive()
        {
            this.m_animationDrive = 1f;
        }

        void Awake()
        {
            if (m_interactableObject == null)
                m_interactableObject = this.transform;
            VG_Controller.OnObjectGrasped.AddListener(OnObjectGrasped);
            VG_Controller.OnObjectReleased.AddListener(OnObjectReleased);
            enabled = false;
        }

        private void OnObjectGrasped(VG_HandStatus status)
        {
            if (status.m_selectedObject != m_interactableObject) return;
            m_holdingHand = status.m_hand;
            this.enabled = true;
        }

        private void OnObjectReleased(VG_HandStatus status)
        {
            if (status.m_selectedObject != m_interactableObject) return;
            if (m_holdingHand == status.m_hand)
            {
                m_holdingHand = null;
                VG_Controller.GetGraspingAvatars(m_interactableObject, out var hands);
                foreach (var hand in hands)
                {
                    m_holdingHand = VG_Controller.GetHand(hand.Key, hand.Value).m_hand;
                }
                this.enabled = m_holdingHand != null;
            }
        }

        void OnEnable()
        {
            m_events.OnAnimationStarted?.Invoke();
        }

        void OnDisable()
        {
            m_events.OnAnimationStopped?.Invoke();
        }

        private void LateUpdate()
        {
            for (int i = 0; i < m_fingerAnimations.Count; i++)
            {
                VG_FingerAnimationData animation = m_fingerAnimations[i];
                if (animation.disabled) continue;

                for (int fingerIndex = 0; fingerIndex < 5; fingerIndex++)
                {
                    Finger fingerEnum = FingerEnumFromIndex(fingerIndex);
                    if (animation.finger.HasFlag(fingerEnum))
                    {
                        for (int boneIndex = 0; boneIndex < 3; boneIndex++)
                        {
                            Bone boneEnum = BoneEnumFromIndex(boneIndex);
                            if (animation.bone.HasFlag(boneEnum))
                            {
                                int avatarID = VG_Controller.GetHand(m_holdingHand).m_avatarID;
                                AnimateFingerBone(avatarID, fingerIndex, boneIndex, animation);
                            }
                        }
                    }
                }
            }
        }

        private void AnimateFingerBone(int avatarID, int fingerIndex, int boneIndex, VG_FingerAnimationData animation)
        {
            if (VG_Controller.GetFingerBone(avatarID, (VG_HandSide)this.m_hand, fingerIndex, boneIndex, out Transform bone) != VG_ReturnCode.SUCCESS)
                return;

            Vector3 startRotationEuler = bone.localRotation.eulerAngles;
            Vector3 targetRotationEuler = startRotationEuler + animation.rotation;
            if (m_startRotation.ContainsKey(bone))
                startRotationEuler = m_startRotation[bone];
            else
                m_startRotation.Add(bone, startRotationEuler);

            if (m_targetRotation.ContainsKey(bone))
                targetRotationEuler = m_targetRotation[bone];
            else
                m_targetRotation.Add(bone, targetRotationEuler);

            Quaternion startRotation = Quaternion.Euler(startRotationEuler);
            Quaternion targetRotation = Quaternion.Euler(targetRotationEuler);
            float targetAnimationDrive = this.m_animationDrive;
            if (animation.ignoreDrive)
            {
                targetAnimationDrive = 1f;
            }
            else if (animation.invertDrive)
            {
                targetAnimationDrive = 1f - targetAnimationDrive;
            }
            
            bone.localRotation = Quaternion.Slerp(startRotation, targetRotation, targetAnimationDrive);
        }
    }

    public partial class VG_FingerAnimator
    {
        public enum HandSide
        {
            LEFT = VG_HandSide.LEFT,
            RIGHT = VG_HandSide.RIGHT
        }
        [Serializable]
        public class VG_FingerAnimationData
        {
            [Tooltip("Disables this animation clip")]
            public bool disabled = true;
            [Tooltip("Makes this animation clip always reach target rotation, effectively ignoring any animation drive")]
            public bool ignoreDrive = false;
            [Tooltip("Makes this animation clip reach target rotation when the animation drive value is 0 rather than 1")]
            public bool invertDrive = false;
            [System.Flags]
            public enum Finger
            {
                Thumb = 1, Index = 2, Middle = 4, Ring = 8, Pinky = 16
            };
            [Tooltip("Fingers to animate in this clip, multiple can be selected")]
            public Finger finger = Finger.Thumb;
            [System.Flags]
            public enum Bone
            {
                Proximal = 1, Middle = 2, Distal = 4
            };
            [Tooltip("Bones to animate in this clip, multiple can be selected")]
            public Bone bone = Bone.Proximal;
            [Tooltip("Target rotation relative to the initial rotation for selected bones")]
            public Vector3 rotation;

            public static Bone BoneEnumFromIndex(int boneIndex)
            {
                return boneIndex switch
                {
                    0 => Bone.Proximal,
                    1 => Bone.Middle,
                    2 => Bone.Distal,
                    _ => throw new ArgumentException($"Couldn't map index {boneIndex} to bone enum")
                };
            }

            public static Finger FingerEnumFromIndex(int fingerIndex)
            {
                return fingerIndex switch
                {
                    0 => Finger.Thumb,
                    1 => Finger.Index,
                    2 => Finger.Middle,
                    3 => Finger.Ring,
                    4 => Finger.Pinky,
                    _ => throw new ArgumentException($"Couldn't map index {fingerIndex} to finger enum")
                };
            }
        }

        [Serializable]
        public class AnimationEvents
        {
            [SerializeField]
            public UnityEvent OnAnimationStarted, OnAnimationStopped;
        }
    }
}
    