// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using UnityEngine.Events;

namespace VirtualGrasp.Tests
{
    public class RuntimeInputTesting : MonoBehaviour
    {
        enum InputMode { None = 0, SphereTouch = 1, ButtonTwoStage = 2 };

        public Transform m_object = null;
        public float m_distance = 0.5f;
        private bool m_inTouch = false;

        public UnityEvent<Transform> OnRuntimeTrigger = new();
        public UnityEvent<Transform> OnRuntimeRelease = new();

        private InputMode m_inputMode = InputMode.None;
        private VG_Articulation m_vgArticulation = null;

        // Start is called before the first frame update
        void Start()
        {
            if (transform.TryGetComponent<VG_Articulation>(out m_vgArticulation)
                && m_vgArticulation.m_type == VG_JointType.PRISMATIC && m_vgArticulation.m_affordances.m_bounces_two_state)
                m_inputMode = InputMode.ButtonTwoStage;
            else
                m_inputMode = InputMode.SphereTouch;

            if (m_inputMode == InputMode.SphereTouch && m_object == null)
            {
                Debug.LogError("Need to assign Object for touching input sphere for runtime input test.");
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_inputMode == InputMode.SphereTouch)
                SphereTouchInvoke();
            else if (m_inputMode == InputMode.ButtonTwoStage)
                ButtonTwoStageInvoke();
        }

        private void SphereTouchInvoke()
        {
            if (!m_inTouch && (transform.position - m_object.position).magnitude <= m_distance)
            {
                m_inTouch = true;
                OnRuntimeTrigger.Invoke(m_object);
            }

            if (m_inTouch && (transform.position - m_object.position).magnitude > m_distance)
            {
                m_inTouch = false;
                OnRuntimeRelease.Invoke(m_object);
            }
        }

        private void ButtonTwoStageInvoke()
        {
            VG_Controller.GetObjectJointState(transform, out float jointState);
            if (!m_inTouch && jointState == m_vgArticulation.m_max)
            {
                m_inTouch = true;
                OnRuntimeTrigger.Invoke(m_object);
            }

            if (m_inTouch && jointState == m_vgArticulation.m_min)
            {
                m_inTouch = false;
                OnRuntimeRelease.Invoke(m_object);
            }
        }

        public void ChangeSphereColorOnTouching()
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }

        public void ChangeSphereColorOffTouching()
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
    }
}