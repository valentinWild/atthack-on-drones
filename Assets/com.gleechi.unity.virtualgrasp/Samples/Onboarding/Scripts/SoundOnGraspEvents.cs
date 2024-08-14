// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using VirtualGrasp;

namespace VirtualGrasp.Onboarding
{
    public class SoundOnGraspEvents : MonoBehaviour
    {
        public AudioSource graspSoundEffect;
        private int avatarID = 0;

        private void Start()
        {
            VG_Controller.GetSensorControlledAvatarID(out avatarID);
            VG_Controller.OnGraspTriggered.AddListener(PlayGraspSound);
        }

        public void PlayGraspSound(VG_HandStatus hand)
        {
            if (hand.m_avatarID == avatarID)
                graspSoundEffect.Play();
        }
    }
}