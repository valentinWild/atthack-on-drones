// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;

namespace VirtualGrasp.Scripts
{
    /** 
     * VG_ObjectAnimator animates a selected object by either rotating around an axis or translate along an axis.
     * Useful for to achieve in-hand manipulation of articulated objects.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_component_vgobjectanimator." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_ObjectAnimator : MonoBehaviour
    {
        [SerializeField, Tooltip("Optional, if not assigned this transform will be used")]
        private Transform m_object;
        [SerializeField, Tooltip("Which axis to rotate around or translate along")]
        private SnapAxis m_axis = SnapAxis.X;
        [SerializeField, Tooltip("How much degree to rotate around the axis")]
        private float m_angle = 0f;
        [SerializeField, Tooltip("How much distance (m) to translate along the axis")]
        private float m_distance = 0f;

        private Vector3 m_initialLocalPosition = Vector3.zero;
        private Quaternion m_initialLocalRotation = Quaternion.identity;

        void Awake()
        {
            if (this.m_object == null)
                this.m_object = transform;
        }
        private void Start()
        {
            m_initialLocalPosition = this.m_object.localPosition;
            m_initialLocalRotation = this.m_object.localRotation;
        }
        /// <summary>
        /// Pass in a float in range [0,1] to drive the translation animation of the object
        /// </summary>
        public void Translate(float inputValue)
        {
            inputValue = Mathf.Clamp01(inputValue);
            float targetDist = 0f;
            float maxDistanceToMove = this.m_distance;
            if (inputValue < 0f)
            {
                inputValue = Mathf.Abs(inputValue);
                maxDistanceToMove *= -1f;
            }
            targetDist = Mathf.Lerp(0f, maxDistanceToMove, inputValue);

            var targetPosition = this.m_object.localPosition;
            switch (this.m_axis)
            {
                case SnapAxis.X:
                    targetPosition.x = targetDist;
                    break;
                case SnapAxis.Y:
                    targetPosition.y = targetDist;
                    break;
                case SnapAxis.Z:
                    targetPosition.z = targetDist;
                    break;
                default:
                    throw new System.NotImplementedException("Axis to rotate not defined");
            }

            this.m_object.localPosition = m_initialLocalPosition + targetPosition;
        }

        /// <summary>
        /// Pass in a float in range [0,1] to drive the rotation animation of the object
        /// </summary>
        public void Rotate(float inputValue)
        {
            inputValue = Mathf.Clamp01(inputValue);

            float targetAngle = 0f;
            float maxDegreesToRotate = this.m_angle;
            if (inputValue < 0f)
            {
                inputValue = Mathf.Abs(inputValue);
                maxDegreesToRotate *= -1f;
            }
            targetAngle = Mathf.Lerp(0f, maxDegreesToRotate, inputValue);

            var targetRotation = this.m_object.localRotation.eulerAngles;
            switch (this.m_axis)
            {
                case SnapAxis.X:
                    targetRotation.x = targetAngle;
                    break;
                case SnapAxis.Y:
                    targetRotation.y = targetAngle;
                    break;
                case SnapAxis.Z:
                    targetRotation.z = targetAngle;
                    break;
                default:
                    throw new System.NotImplementedException("Axis to rotate not defined");
            }

            this.m_object.localRotation = Quaternion.Euler(targetRotation) * m_initialLocalRotation;
        }
    }
}