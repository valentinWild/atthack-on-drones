// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;

namespace VirtualGrasp.Onboarding
{
    public class SimpleImpact : MonoBehaviour
    {
        public AudioSource collisionEffect;

        private float unityVelocityThreshold = 1.0F;
        private float calculatedVelocityThreshold = .1F;

        ArticulationBody ownAB = null;
        Rigidbody ownRB = null;

        private void Start()
        {
            gameObject.TryGetComponent<Rigidbody>(out ownRB);
            gameObject.TryGetComponent<ArticulationBody>(out ownAB);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > 0) // sometimes we're lucky. unity calculates speed for us
            {
                if (collision.relativeVelocity.magnitude > unityVelocityThreshold)
                    collisionEffect.Play();
            }
            else if (
#if UNITY_2020_3_16_OR_NEWER
            collision.body != null &&
#endif
                (ownRB != null || ownAB != null))
            { // sometimes we're not lucky, have to calculate
                Vector3 ownSpeed = ownRB != null ? ownRB.velocity : ownAB.velocity;
                Vector3 colliderSpeed = Vector3.zero;
#if UNITY_2020_3_16_OR_NEWER
            if (collision.rigidbody != null) colliderSpeed = collision.rigidbody.velocity;
            else if (collision.articulationBody != null) colliderSpeed = collision.articulationBody.velocity;
#else
                if (collision.rigidbody != null) colliderSpeed = collision.rigidbody.velocity;
#endif

                if ((ownSpeed - colliderSpeed).magnitude > calculatedVelocityThreshold)
                    collisionEffect.Play();
            }
        }
    }
}