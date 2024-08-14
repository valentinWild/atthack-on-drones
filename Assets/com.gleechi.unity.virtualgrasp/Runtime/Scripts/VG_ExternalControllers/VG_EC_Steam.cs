// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

//#define VG_USE_STEAMVR_CONTROLLER

#if UNITY_ANDROID && !UNITY_EDITOR // SteamVR is not supported on Android
#undef VG_USE_STEAMVR_CONTROLLER
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

#if VG_USE_STEAMVR_CONTROLLER
using Valve.VR;
#endif

namespace VirtualGrasp.Controllers
{
	/**
     * This is an external controller class that supports the Steam Knuckles controller as an external controller.
     * Please refer to https://docs.virtualgrasp.com/controllers.html for the definition of an external controller for VG.
     * 
     * The following requirements have to be met to be able to enable the #define VG_USE_STEAMVR_CONTROLLER above and use the controller:
     * - You have the SteamVR Unity plugin from https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647 imported into your Unity project.
     * - You have Steam and the corresponding SteamVR SDK (https://store.steampowered.com/app/250820/SteamVR/) installed on your computer.
     * - You have OpenVR Loader selected in Unity XR Management Project Settings.
     */

	[LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_vg_ec_steam." + VG_Version.__VG_VERSION__ + ".html")]

    public class VG_EC_Steam : VG_ExternalController
	{
#if VG_USE_STEAMVR_CONTROLLER
	private SteamVR_Action_Skeleton m_skeleton = null;
	private bool m_skeletonActive = false;
#endif

		[Serializable]
		public class SteamHandMapping : VG_BoneMapping
		{
			public override void Initialize(int avatarID, VG_HandSide side)
			{
				base.Initialize(avatarID, side);
				m_BoneToTransform = new Dictionary<int, Transform>()
				{
#if VG_USE_STEAMVR_CONTROLLER
				{ SteamVR_Skeleton_JointIndexes.root, null },
				{ SteamVR_Skeleton_JointIndexes.wrist, Hand_WristRoot },
				{ SteamVR_Skeleton_JointIndexes.thumbProximal, Hand_Thumb1 },
				{ SteamVR_Skeleton_JointIndexes.thumbMiddle, Hand_Thumb2 },
				{ SteamVR_Skeleton_JointIndexes.thumbDistal, Hand_Thumb3 },
				{ SteamVR_Skeleton_JointIndexes.thumbTip, null },
				{ SteamVR_Skeleton_JointIndexes.indexMetacarpal, null },
				{ SteamVR_Skeleton_JointIndexes.indexProximal, Hand_Index1 },
				{ SteamVR_Skeleton_JointIndexes.indexMiddle, Hand_Index2 },
				{ SteamVR_Skeleton_JointIndexes.indexDistal, Hand_Index3 },
				{ SteamVR_Skeleton_JointIndexes.indexTip, null },
				{ SteamVR_Skeleton_JointIndexes.middleMetacarpal, null },
				{ SteamVR_Skeleton_JointIndexes.middleProximal, Hand_Middle1 },
				{ SteamVR_Skeleton_JointIndexes.middleMiddle, Hand_Middle2 },
				{ SteamVR_Skeleton_JointIndexes.middleDistal, Hand_Middle3 },
				{ SteamVR_Skeleton_JointIndexes.middleTip, null },
				{ SteamVR_Skeleton_JointIndexes.ringMetacarpal, null },
				{ SteamVR_Skeleton_JointIndexes.ringProximal, Hand_Ring1 },
				{ SteamVR_Skeleton_JointIndexes.ringMiddle, Hand_Ring2 },
				{ SteamVR_Skeleton_JointIndexes.ringDistal, Hand_Ring3 },
				{ SteamVR_Skeleton_JointIndexes.ringTip, null },
				{ SteamVR_Skeleton_JointIndexes.pinkyMetacarpal, null },
				{ SteamVR_Skeleton_JointIndexes.pinkyProximal, Hand_Pinky1 },
				{ SteamVR_Skeleton_JointIndexes.pinkyMiddle, Hand_Pinky2 },
				{ SteamVR_Skeleton_JointIndexes.pinkyDistal, Hand_Pinky3 },
				{ SteamVR_Skeleton_JointIndexes.pinkyTip, null }
#endif
				};

				m_BoneToParent = new Dictionary<int, int>()
				{
				};
#if VG_USE_STEAMVR_CONTROLLER
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.wrist] = SteamVR_Skeleton_JointIndexes.root;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.thumbProximal] = SteamVR_Skeleton_JointIndexes.wrist;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.thumbMiddle] = SteamVR_Skeleton_JointIndexes.thumbProximal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.thumbDistal] = SteamVR_Skeleton_JointIndexes.thumbMiddle;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.thumbTip] = SteamVR_Skeleton_JointIndexes.thumbDistal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.indexMetacarpal] = SteamVR_Skeleton_JointIndexes.wrist;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.indexProximal] = SteamVR_Skeleton_JointIndexes.indexMetacarpal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.indexMiddle] = SteamVR_Skeleton_JointIndexes.indexProximal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.indexDistal] = SteamVR_Skeleton_JointIndexes.indexMiddle;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.indexTip] = SteamVR_Skeleton_JointIndexes.indexDistal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.middleMetacarpal] = SteamVR_Skeleton_JointIndexes.wrist;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.middleProximal] = SteamVR_Skeleton_JointIndexes.middleMetacarpal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.middleMiddle] = SteamVR_Skeleton_JointIndexes.middleProximal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.middleDistal] = SteamVR_Skeleton_JointIndexes.middleMiddle;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.middleTip] = SteamVR_Skeleton_JointIndexes.middleDistal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.ringMetacarpal] = SteamVR_Skeleton_JointIndexes.wrist;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.ringProximal] = SteamVR_Skeleton_JointIndexes.ringMetacarpal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.ringMiddle] = SteamVR_Skeleton_JointIndexes.ringProximal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.ringDistal] = SteamVR_Skeleton_JointIndexes.ringMiddle;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.ringTip] = SteamVR_Skeleton_JointIndexes.ringDistal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.pinkyMetacarpal] = SteamVR_Skeleton_JointIndexes.wrist;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.pinkyProximal] = SteamVR_Skeleton_JointIndexes.pinkyMetacarpal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.pinkyMiddle] = SteamVR_Skeleton_JointIndexes.pinkyProximal;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.pinkyDistal] = SteamVR_Skeleton_JointIndexes.pinkyMiddle;
			m_BoneToParent[SteamVR_Skeleton_JointIndexes.pinkyTip] = SteamVR_Skeleton_JointIndexes.pinkyDistal;
#endif
			}
		}

		public VG_EC_Steam(int avatarID, VG_HandSide side, Transform origin)
		{
			m_avatarID = avatarID;
			m_handType = side;
			m_origin = origin;
			m_enablingDefine = "VG_USE_STEAMVR_CONTROLLER";

#if VG_USE_STEAMVR_CONTROLLER
			m_enabled = true;
#else
			PrintNotEnabledError();
			m_enabled = false;
#endif
		}

		public new void Initialize()
		{
#if VG_USE_STEAMVR_CONTROLLER
		m_mapping = new SteamHandMapping();
		base.Initialize();

		SteamVR.Initialize();
		m_skeleton = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>(m_handType == VG_HandSide.LEFT ? "SkeletonLeftHand" : "SkeletonRightHand");
		m_skeleton.SetRangeOfMotion(EVRSkeletalMotionRange.WithoutController);
		//m_skeleton.SetSkeletalTransformSpace(Valve.VR.EVRSkeletalTransformSpace.Model);

		m_initialized = true;
		if (GetNumBones() == 0)
		{
			Debug.LogError("Could not find bone skeleton root (boneCount=" + GetNumBones() + "; skeleton active=" + m_skeletonActive + ").");
			m_initialized = false;
		}
#endif
		}

		public override float GetGrabStrength()
		{
#if VG_USE_STEAMVR_CONTROLLER
		if (m_skeleton == null) return 0.0f;
		return (m_skeleton.fingerCurls[0] + m_skeleton.fingerCurls[1] + m_skeleton.fingerCurls[2]) / 3.0f;
#else
			return 0.0f;
#endif
		}

		public override bool Compute()
		{

#if VG_USE_STEAMVR_CONTROLLER
		if (!m_enabled) return false;
		if (!m_initialized) { Initialize(); return false; }

		ETrackingResult res = m_skeleton.GetTrackingResult(m_handType == VG_HandSide.LEFT ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand);
		if (res == 0) return false;
		m_skeletonActive = m_skeleton.GetActive();
		
		for (int boneId = 0; boneId < (m_skeletonActive ? GetNumBones() : 2); ++boneId)
		{
			if (boneId == 0) // root
			{
				SetPose(boneId, Matrix4x4.identity);
			}
			else if (boneId == 1) // wrist
			{
				SetPose(boneId, Matrix4x4.TRS(m_skeleton.GetLocalPosition(), m_skeleton.GetLocalRotation(), Vector3.one));
			}
			else
			{
				SetPose(boneId, m_poses[m_mapping.GetParent(boneId)] * // When transform space is local / Parent
					Matrix4x4.TRS(m_skeleton.bonePositions[boneId], m_skeleton.boneRotations[boneId], Vector3.one));
			}
		}
#endif

			return true;
		}

		public override void HapticPulse(VG_HandStatus hand, float amplitude = 0.5F, float duration = 0.01F, int finger = 5)
		{
#if VG_USE_STEAMVR_CONTROLLER
		SteamVR_Actions.default_Haptic[hand.m_side == VG_HandSide.LEFT ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand].Execute(0, duration, 10, amplitude);
#endif
		}

		public override Color GetConfidence()
		{
#if VG_USE_STEAMVR_CONTROLLER
		EVRSkeletalTrackingLevel skeletalTrackingLevel = EVRSkeletalTrackingLevel.VRSkeletalTracking_Estimated;
		if (m_skeleton == null || OpenVR.Input.GetSkeletalTrackingLevel(m_skeleton.handle, ref skeletalTrackingLevel) != EVRInputError.None)
			return Color.black;

		switch (skeletalTrackingLevel)
		{
			case EVRSkeletalTrackingLevel.VRSkeletalTracking_Full:
				return Color.green;
			case EVRSkeletalTrackingLevel.VRSkeletalTracking_Partial:
				return Color.yellow;
			case EVRSkeletalTrackingLevel.VRSkeletalTracking_Estimated:
				return Color.red;
		}
#endif
			return Color.black;
		}
	}
}