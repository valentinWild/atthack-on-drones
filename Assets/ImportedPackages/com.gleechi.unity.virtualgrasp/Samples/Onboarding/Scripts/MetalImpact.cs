// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;

namespace VirtualGrasp.Onboarding
{
    public class MetalImpact : MonoBehaviour
    {
        public AudioSource plateCollisionEffect;
        public AudioSource tableCollisionEffect;

        private float verticalVelocityThreshold = 1.0F;

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.gameObject.name.Contains("plate"))
            {
                if (collision.relativeVelocity.magnitude > verticalVelocityThreshold)
                {
                    plateCollisionEffect.Play();
                }

            }
            else if (collision.gameObject.name.Contains("SM_Factory1001"))
            {
                if (collision.relativeVelocity.magnitude > verticalVelocityThreshold)
                {
                    tableCollisionEffect.Play();
                }
            }
        }
    }
}