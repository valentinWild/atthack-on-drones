// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using UnityEngine.XR;

namespace VirtualGrasp.Scripts
{
    /** 
     * VG_Locomotion does not depends on VG library, and is a script that provides a convenient tool for locomation.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_component_vglocomotion." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_Locomotion : MonoBehaviour
    {
        public Transform m_character = null;
        private Vector2 m_axisL = Vector2.zero;
        private Vector2 m_axisR = Vector2.zero;
        private Camera m_camera = null;

        public float speed = 1.0f;
        public float rotationSpeed = 60f;

        void TryAssignCamera()
        {
            if (m_character == null) m_character = transform;
            m_camera = GetComponentInChildren<Camera>();
            if (m_camera == null) m_camera = Camera.main;
        }

        void FixedUpdate()
        {
            if (m_camera == null)
            {
                TryAssignCamera();
                return;
            }

            if (!InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out m_axisL))
                m_axisL = Vector2.zero;

            if (!InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out m_axisR))
                m_axisR = Vector2.zero;

            // Key board control
            if (Input.GetKey(KeyCode.W))
            {
                m_axisL.y += 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                m_axisL.y -= 1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                m_axisL.x -= 1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                m_axisL.x += 1f;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                m_character.transform.Translate(new Vector3(0f, -speed * Time.deltaTime, 0f));
            }
            if (Input.GetKey(KeyCode.E))
            {
                m_character.transform.Translate(new Vector3(0f, speed * Time.deltaTime, 0f));
            }

            // Joint stick control from controllers

            //float x = Mathf.Abs(m_axisL.x) > Mathf.Abs(m_axisR.x) ? m_axisL.x : m_axisR.x;
            //float y = Mathf.Abs(m_axisL.y) > Mathf.Abs(m_axisR.y) ? m_axisL.y : m_axisR.y;
            //if (Mathf.Abs(x) > 0.1f) m_character.Rotate(new Vector3(0, 2.0f * x, 0), Space.Self);
            //if (Mathf.Abs(y) > 0.1f) m_character.Translate(0.03f * y * (m_camera.transform.rotation * Vector3.forward), Space.World);

            // Below has speed control
            float x = Mathf.Abs(m_axisL.x) > Mathf.Abs(m_axisR.x) ? m_axisL.x : m_axisR.x;
            float y = Mathf.Abs(m_axisL.y) > Mathf.Abs(m_axisR.y) ? m_axisL.y : m_axisR.y;
            m_character.Rotate(new Vector3(0, rotationSpeed * x * Time.deltaTime, 0), Space.Self);
            m_character.Translate(speed * y * Time.deltaTime * (m_camera.transform.rotation * Vector3.forward), Space.World);
        }


    }
}