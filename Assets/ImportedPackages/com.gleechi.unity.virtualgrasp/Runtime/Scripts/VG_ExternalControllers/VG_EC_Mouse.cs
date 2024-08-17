// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
#if VG_ENABLE_INPUT_SYSTEM // && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace VirtualGrasp.Controllers
{
    /**
     * This is an external controller class that supports a Mouse controller as an external controller.
     * Please refer to https://docs.virtualgrasp.com/controllers.html for the definition of an external controller for VG.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_vg_ec_mouse." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_EC_Mouse : VG_ExternalController
    {
        private int mouse_held = 0;
        private int filter = 15;
        private float depth = .5f;
        private Vector3 rotation = Vector3.zero;
#if VG_ENABLE_INPUT_SYSTEM
        private float rotationSpeed = 0.5f;
#endif

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

        public VG_EC_Mouse(int avatarID, VG_HandSide side, Transform origin)
        {
            m_avatarID = avatarID;
            m_handType = side;
            m_origin = origin;

            m_enabled = true;
        }

        public new void Initialize()
        {
            m_initialized = (Camera.main != null);
            if (!m_initialized) return;

            m_mapping = new HandMapping();
            base.Initialize();
            if (Camera.main.stereoTargetEye != StereoTargetEyeMask.None)
            {
                Debug.LogWarning("VG_EC_MouseHand uses single GameView camera, but a stereo camera is activated. Deactivating Stereo view.");
                Camera.main.stereoTargetEye = StereoTargetEyeMask.None;
            }
        }

        public override bool Compute()
        {
            if (!m_enabled || !Application.isFocused) return false;
            if (!m_initialized || m_mapping == null) { Initialize(); return false; }
            
#if VG_ENABLE_INPUT_SYSTEM // && !ENABLE_LEGACY_INPUT_MANAGER
            if (Keyboard.current.leftShiftKey.isPressed && m_handType == VG_HandSide.RIGHT ||
                Keyboard.current.rightShiftKey.isPressed && m_handType == VG_HandSide.LEFT)
                return true;

            if (Keyboard.current.xKey.IsPressed()) rotation.x += rotationSpeed;
            if (Keyboard.current.yKey.IsPressed()) rotation.y += rotationSpeed;
            if (Keyboard.current.zKey.IsPressed()) rotation.z += rotationSpeed;

            if (m_handType == VirtualGrasp.VG_HandSide.LEFT ? Mouse.current.leftButton.isPressed : Mouse.current.rightButton.isPressed)
            mouse_held = Mathf.Min(filter, mouse_held + 1);
            else mouse_held = Mathf.Max(0, mouse_held - 1);
            depth = Mathf.Clamp(depth + Mouse.current.scroll.y.ReadValue() / 100.0f, .01f, 1.0f);
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
            if (Input.GetMouseButton(m_handType == VirtualGrasp.VG_HandSide.LEFT ? 0 : 1)) mouse_held = Mathf.Min(filter, mouse_held + 1);
            else mouse_held = Mathf.Max(0, mouse_held - 1);
            depth = Mathf.Clamp(depth + Input.mouseScrollDelta.y / 10.0f, .5f, 3.0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif
            Quaternion q = Camera.main.transform.rotation;
            Vector3 p = ray.origin + depth * ray.direction + q * (m_handType == VirtualGrasp.VG_HandSide.LEFT ? Vector3.left : Vector3.right) * .1f;
            q = q * Quaternion.Euler(rotation);
            SetPose(0, Matrix4x4.TRS(p, q, Vector3.one));
            return true;
        }

        public override float GetGrabStrength()
        {
            return (float)mouse_held / filter;
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