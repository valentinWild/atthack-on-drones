// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

//#define VG_USE_XRHANDS_CONTROLLER

using System;
using System.Collections.Generic;
using UnityEngine;
#if VG_USE_XRHANDS_CONTROLLER
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
#endif

namespace VirtualGrasp.Controllers
{
    /**
     * This is an external controller class that supports the Unity XRHands controller as an external controller.
     * Please refer to https://docs.virtualgrasp.com/controllers.html for the definition of an external controller for VG.
     * 
     * The following requirements have to be met to be able to enable the #define VG_USE_XRHANDS_CONTROLLER above and use the controller:
     * - You have followed the installation instructions of the Unity XRHands package,
     *   from https://docs.unity3d.com/Packages/com.unity.xr.hands@1.1/manual/
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_vg_ec_unityxrhands." + VG_Version.__VG_VERSION__ + ".html")]

    public class VG_EC_UnityXRHands : VG_ExternalController
    {
#if VG_USE_XRHANDS_CONTROLLER
    XRHandSubsystem m_Subsystem = null;
    XRHand m_hand;
    XRHandSubsystem.UpdateSuccessFlags m_updateSuccessFlags = XRHandSubsystem.UpdateSuccessFlags.None;
#endif

        [Serializable]
        public class OpenXRHandMapping : VG_BoneMapping
        {
            public override void Initialize(int avatarID, VG_HandSide side)
            {
                base.Initialize(avatarID, side);
                m_BoneToTransform = new Dictionary<int, Transform>()
                {
#if VG_USE_XRHANDS_CONTROLLER
                { XRHandJointID.Wrist.ToIndex(), Hand_WristRoot },
                { XRHandJointID.Palm.ToIndex(), null },
                { XRHandJointID.ThumbMetacarpal.ToIndex(), Hand_Thumb1 },
                { XRHandJointID.ThumbProximal.ToIndex(), Hand_Thumb2 },                
                { XRHandJointID.ThumbDistal.ToIndex(), Hand_Thumb3 },
                { XRHandJointID.ThumbTip.ToIndex(), null },
                { XRHandJointID.IndexMetacarpal.ToIndex(), null },
                { XRHandJointID.IndexProximal.ToIndex(), Hand_Index1 },
                { XRHandJointID.IndexIntermediate.ToIndex(), Hand_Index2 },
                { XRHandJointID.IndexDistal.ToIndex(), Hand_Index3 },
                { XRHandJointID.IndexTip.ToIndex(), null },
                { XRHandJointID.MiddleMetacarpal.ToIndex(), null },
                { XRHandJointID.MiddleProximal.ToIndex(), Hand_Middle1 },
                { XRHandJointID.MiddleIntermediate.ToIndex(), Hand_Middle2 },
                { XRHandJointID.MiddleDistal.ToIndex(), Hand_Middle3 },
                { XRHandJointID.MiddleTip.ToIndex(), null },
                { XRHandJointID.RingMetacarpal.ToIndex(), null },
                { XRHandJointID.RingProximal.ToIndex(), Hand_Ring1 },
                { XRHandJointID.RingIntermediate.ToIndex(), Hand_Ring2 },
                { XRHandJointID.RingDistal.ToIndex(), Hand_Ring3 },
                { XRHandJointID.RingTip.ToIndex(), null },
                { XRHandJointID.LittleMetacarpal.ToIndex(), null },
                { XRHandJointID.LittleProximal.ToIndex(), Hand_Pinky1 },
                { XRHandJointID.LittleIntermediate.ToIndex(), Hand_Pinky2 },
                { XRHandJointID.LittleDistal.ToIndex(), Hand_Pinky3 },
                { XRHandJointID.LittleTip.ToIndex(), null }
#endif
                };

                m_BoneToParent = new Dictionary<int, int>()
                {
                };

#if VG_USE_XRHANDS_CONTROLLER
            m_BoneToParent[XRHandJointID.Palm.ToIndex()] = XRHandJointID.Wrist.ToIndex();
            m_BoneToParent[XRHandJointID.ThumbMetacarpal.ToIndex()] = XRHandJointID.Wrist.ToIndex();
            m_BoneToParent[XRHandJointID.ThumbProximal.ToIndex()] = XRHandJointID.ThumbMetacarpal.ToIndex();
            m_BoneToParent[XRHandJointID.ThumbDistal.ToIndex()] = XRHandJointID.ThumbProximal.ToIndex();
            m_BoneToParent[XRHandJointID.ThumbTip.ToIndex()] = XRHandJointID.ThumbDistal.ToIndex();
            m_BoneToParent[XRHandJointID.IndexMetacarpal.ToIndex()] = XRHandJointID.Wrist.ToIndex();
            m_BoneToParent[XRHandJointID.IndexProximal.ToIndex()] = XRHandJointID.IndexMetacarpal.ToIndex();
            m_BoneToParent[XRHandJointID.IndexIntermediate.ToIndex()] = XRHandJointID.IndexProximal.ToIndex();
            m_BoneToParent[XRHandJointID.IndexDistal.ToIndex()] = XRHandJointID.IndexIntermediate.ToIndex();
            m_BoneToParent[XRHandJointID.IndexTip.ToIndex()] = XRHandJointID.IndexDistal.ToIndex();
            m_BoneToParent[XRHandJointID.MiddleMetacarpal.ToIndex()] = XRHandJointID.Wrist.ToIndex();
            m_BoneToParent[XRHandJointID.MiddleProximal.ToIndex()] = XRHandJointID.MiddleMetacarpal.ToIndex();
            m_BoneToParent[XRHandJointID.MiddleIntermediate.ToIndex()] = XRHandJointID.MiddleProximal.ToIndex();
            m_BoneToParent[XRHandJointID.MiddleDistal.ToIndex()] = XRHandJointID.MiddleIntermediate.ToIndex();
            m_BoneToParent[XRHandJointID.MiddleTip.ToIndex()] = XRHandJointID.MiddleDistal.ToIndex();
            m_BoneToParent[XRHandJointID.RingMetacarpal.ToIndex()] = XRHandJointID.Wrist.ToIndex();
            m_BoneToParent[XRHandJointID.RingProximal.ToIndex()] = XRHandJointID.RingMetacarpal.ToIndex();
            m_BoneToParent[XRHandJointID.RingIntermediate.ToIndex()] = XRHandJointID.RingProximal.ToIndex();
            m_BoneToParent[XRHandJointID.RingDistal.ToIndex()] = XRHandJointID.RingIntermediate.ToIndex();
            m_BoneToParent[XRHandJointID.RingTip.ToIndex()] = XRHandJointID.RingDistal.ToIndex();
            m_BoneToParent[XRHandJointID.LittleMetacarpal.ToIndex()] = XRHandJointID.Wrist.ToIndex();
            m_BoneToParent[XRHandJointID.LittleProximal.ToIndex()] = XRHandJointID.LittleMetacarpal.ToIndex();
            m_BoneToParent[XRHandJointID.LittleIntermediate.ToIndex()] = XRHandJointID.LittleProximal.ToIndex();
            m_BoneToParent[XRHandJointID.LittleDistal.ToIndex()] = XRHandJointID.LittleIntermediate.ToIndex();
            m_BoneToParent[XRHandJointID.LittleTip.ToIndex()] = XRHandJointID.LittleDistal.ToIndex();

#endif
            }
        }

        public VG_EC_UnityXRHands(int avatarID, VG_HandSide side, Transform origin)
        {
            m_avatarID = avatarID;
            m_handType = side;
            m_origin = origin;
            m_enablingDefine = "VG_USE_XRHANDS_CONTROLLER";

#if VG_USE_XRHANDS_CONTROLLER
            m_enabled = true;
#else
            PrintNotEnabledError();
            m_enabled = false;
#endif
        }

        public new void Initialize()
        {
#if VG_USE_XRHANDS_CONTROLLER
        m_mapping = new OpenXRHandMapping();		
        base.Initialize();
        
        m_Subsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRHandSubsystem>();
        if (m_Subsystem != null)
        {
            if (m_handType == VG_HandSide.LEFT)
            {
                m_hand = m_Subsystem.leftHand;
                m_updateSuccessFlags = XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose | XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints;
            }
            else
            {
                m_hand = m_Subsystem.rightHand;
                m_updateSuccessFlags = XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose | XRHandSubsystem.UpdateSuccessFlags.RightHandJoints;
            }

            if (!m_Subsystem.running) m_Subsystem.Start();

            base.Initialize();			
            m_initialized = true;
        }
        else m_initialized = false;
#endif
        }

        public override float GetGrabStrength()
        {
            return -1.0f; // let VG decide from full DOF
        }

        public override bool Compute()
        {
            if (!m_enabled) return false;
            if (!m_initialized) { Initialize(); return false; }
#if VG_USE_XRHANDS_CONTROLLER
        if (!m_Subsystem.running) return false;
        if (!m_Subsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic).HasFlag(m_updateSuccessFlags))
            return false;

        for (int boneId = 0; boneId < GetNumBones(); ++boneId)
        {
            if (m_hand.GetJoint(XRHandJointIDUtility.FromIndex(boneId)).TryGetPose(out Pose pose))
            {
                //if (m_origin != null) pose = pose.GetTransformedBy(m_origin);
                SetPose(boneId, Matrix4x4.TRS(
                    pose.position, 
                    pose.rotation, Vector3.one));                            
            }
        }

        return true;
#else
            return false;
#endif
        }

        public override void HapticPulse(VG_HandStatus hand, float amplitude = 0.5F, float duration = 0.01F, int finger = 5)
        {
        }

        public override Color GetConfidence()
        {
            if (!m_initialized) return Color.black;

            return Color.yellow;
        }
    }
}