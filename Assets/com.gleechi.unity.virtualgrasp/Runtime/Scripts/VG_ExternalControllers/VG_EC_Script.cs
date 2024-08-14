// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace VirtualGrasp.Controllers
{
    /**
     * This is an external controller class that supports a Script/GUI controller as an external controller.
     * Please refer to https://docs.virtualgrasp.com/controllers.html for the definition of an external controller for VG.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_vg_ec_script." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_EC_Script : VG_ExternalController
    {
        private Transform m_wrist = null;
        private float m_grabStrength = 0.0f;

        /**
         * The wrist transform strength can be set and received from another Script/GUI.
         * Changes in the wrist transform will affect the controller through Compute().
         */
        public Transform WristTransform
        {
            get { return m_wrist; }
            set { m_wrist = value; }
        }

        /**
         * The grab strength can be set and received from another Script/GUI.
         * Changes in the grab strength will affect the controller through GetGrabStrength().
         */
        public float GrabStrength
        {
            get { return m_grabStrength; }
            set { m_grabStrength = Mathf.Clamp01(value); }
        }

        [Serializable]
        public class HandMapping : VG_BoneMapping
        {
            public override void Initialize(int avatarID, VG_HandSide side)
            {
                base.Initialize(avatarID, side);
                m_BoneToTransform = new Dictionary<int, Transform>()
                {
                    { 0, Hand_WristRoot }
                };
            }
        }

        public VG_EC_Script(int avatarID, VG_HandSide side, Transform origin)
        {
            m_avatarID = avatarID;
            m_handType = side;
            m_origin = origin;
            m_enabled = true;
            Initialize();
        }

        public new void Initialize()
        {
            m_mapping = new HandMapping();
            base.Initialize();
        }

        public override bool Compute()
        {
            if (!m_enabled || m_wrist == null) return false;
            SetPose(0, Matrix4x4.TRS(m_wrist.position, m_wrist.rotation, Vector3.one));
            return true;
        }

        public override float GetGrabStrength()
        {
            return m_grabStrength;
        }

        public override Color GetConfidence()
        {
            return Color.yellow;
        }

        public override void HapticPulse(VG_HandStatus hand, float amplitude = 0.5F, float duration = 0.015F, int finger = 5)
        {
        }
    }
}