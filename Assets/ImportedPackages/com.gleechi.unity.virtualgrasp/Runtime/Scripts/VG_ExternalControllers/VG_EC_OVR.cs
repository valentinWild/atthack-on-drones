// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

//#define VG_USE_OVRHAND_CONTROLLER // Please read below instructions and requirements before activating.

using UnityEngine;

namespace VirtualGrasp.Controllers 
{
    /**
     * NOTE: this is an experimental controller only for the OculusIntegration sample.
     *       We recommend to not use OVRHand / OVRCustomSkeleton but use one of the 
     *       various finger controllers that come with VirtualGrasp. 
     *
     * This is an external controller class that supports a generic overlay controller for the OVRHand class as an external controller.
     * Please refer to https://docs.virtualgrasp.com/controllers.html for the definition of an external controller for VG.
     * 
     * The following requirements have to be met to be able to enable the #define VG_USE_OVRHAND_CONTROLLER above and use the controller:
     * - You have the Oculus SDK (https://www.oculus.com/setup/) installed on your computer.
     * - You have the Oculus Integration plugin from https://developer.oculus.com/downloads/package/unity-integration/ imported into your Unity project.
     * - You are using a handmodel / rig that is based on the OVRHand / OVRCustomSkeleton classes.
     */

    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_vg_ec_ovr." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_EC_OVR : VG_ExternalController 
    {
#if VG_USE_OVRHAND_CONTROLLER
        private OVRCustomSkeleton m_skeleton = null;
        private OVRHand m_hand = null;
#endif

        public VG_EC_OVR(int avatarID, VG_HandSide side, Transform origin)
        {
            m_avatarID = avatarID;
            m_handType = side;            
            m_zeroOffsets = true; // the generic hand works on the Unity transforms, so it can't use offsets.
            m_origin = origin;
            m_enablingDefine = "VG_USE_OVRHAND_CONTROLLER";

#if VG_USE_OVRHAND_CONTROLLER
            Initialize();
			m_enabled = true;
#else
            PrintNotEnabledError();
            m_enabled = false;
#endif
        }

        public new void Initialize()
        {
            m_mapping = new VG_EC_Oculus.HandMapping();

#if VG_USE_OVRHAND_CONTROLLER
            foreach (OVRCustomSkeleton hand in GameObject.FindObjectsOfType<OVRCustomSkeleton>()) {
                if (hand.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft && m_handType == VG_HandSide.LEFT ||
                    hand.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight && m_handType == VG_HandSide.RIGHT) {
                    m_skeleton = hand;
                    m_hand = hand.gameObject.GetComponent<OVRHand>();
                }
            }

            if (m_skeleton == null) 
            {
                m_initialized = false;
                return;
            }

            base.Initialize();
            m_initialized = true;
#else
            m_initialized = false;
#endif
        }

        public override bool Compute() 
        {
            if (!m_enabled) return false;
            if (!m_initialized) { Initialize(); return false; }

#if VG_USE_OVRHAND_CONTROLLER
            if (!m_skeleton.IsDataValid) return false;
            int offset = (int)m_skeleton.GetCurrentStartBoneId();
            for (int bone = 0; bone < m_mapping.GetNumBones(); bone++) 
            {
                Transform pose = (bone != 1) ?
                    m_skeleton.CustomBones[offset + bone].transform :
                    m_skeleton.CustomBones[offset].transform; // we map forearm stub to wrist                
                SetPose(bone, Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one));
            }
            
            return true;
#else
            return false;
#endif
        }

        public override float GetGrabStrength() {

#if VG_USE_OVRHAND_CONTROLLER
            return -1.0f;
            //return m_hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
#else
            return -1.0f;
#endif
        }

        public override Color GetConfidence()
        {
            return Color.yellow;
        }

        public override void HapticPulse(VG_HandStatus hand, float amplitude = 0.5F, float duration = 0.015F, int finger = 5) {
        }
    }
}