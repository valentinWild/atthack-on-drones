// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;

namespace VirtualGrasp.Onboarding
{
    public class SoundOnPushEvents : MonoBehaviour
    {
        public AudioSource pushSoundEffect;

        // Start is called before the first frame update
        void Start()
        {
            VG_Controller.OnObjectPushed.AddListener(PlayPushSound);
        }

        public void PlayPushSound(VG_HandStatus hand)
        {
            if (hand.m_selectedObject != null && hand.m_selectedObject == transform)
                pushSoundEffect.Play();
        }
    }
}