// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;

namespace VirtualGrasp.Onboarding
{
    /** 
     * ToggleLight shows as a tutorial on a non-physical two-stage button setup 
     * through VG_Articulation and how to use VG_Controller.GetObjectJointState to toggle light on and off. 
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_vgonboarding_task1.html")]
    public class ToggleLight : MonoBehaviour
    {
        public GameObject m_light = null;
        public GameObject m_light2 = null;
        private VG_Articulation m_articulation = null;

        public AudioSource m_audioSourceOn;
        public AudioSource m_audioSourceOff;

        private float m_state;
        private bool m_binState = false;


        void Start()
        {
            if (TryGetComponent<VG_Articulation>(out m_articulation))
            {
                VG_Controller.GetObjectJointState(transform, out m_state);
                EvaluateState(m_state);
            }
        }

        void Update()
        {
            VG_Controller.GetObjectJointState(transform, out float newState);
            if (newState != m_state)
                EvaluateState(newState);
            m_state = newState;
        }

        void EvaluateState(float state)
        {
            if (state == m_articulation.m_min && m_binState)
            {
                m_binState = false;
                m_light.SetActive(false);
                m_light2.SetActive(false);
                m_audioSourceOff.Play();
            }
            else if (state == m_articulation.m_max && !m_binState)
            {
                m_binState = true;
                m_light.SetActive(true);
                m_light2.SetActive(true);
                m_audioSourceOn.Play();
            }
        }
    }
}