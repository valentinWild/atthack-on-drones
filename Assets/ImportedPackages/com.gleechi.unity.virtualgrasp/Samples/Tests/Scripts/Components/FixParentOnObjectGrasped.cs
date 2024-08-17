// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;

namespace VirtualGrasp.Tests
{
    public class FixParentOnObjectGrasped : MonoBehaviour
    {
        public Transform m_fixedObject = null;

        // Start is called before the first frame update
        void Start()
        {
            VG_Controller.OnObjectGrasped.AddListener(FixParent);
            VG_Controller.OnObjectFullyReleased.AddListener(RecoverParent);
        }

        void FixParent(VG_HandStatus hand)
        {
            if (m_fixedObject == null)
            {
                Debug.LogError("Fixed Object need to be set!");
                return;
            }
            if (hand.m_selectedObject == this.transform)
            {
                VG_Controller.GetObjectJointType(m_fixedObject, false, out VG_JointType jointType);
                if (jointType != VG_JointType.FIXED)
                {
                    VG_Controller.ChangeObjectJoint(m_fixedObject, VG_JointType.FIXED);
                }
            }
        }

        void RecoverParent(VG_HandStatus hand)
        {
            if (m_fixedObject == null)
            {
                Debug.LogError("Fixed Object need to be set!");
                return;
            }
            if (hand.m_selectedObject == this.transform)
            {
                VG_Controller.GetObjectJointType(m_fixedObject, false, out VG_JointType jointType);
                if (jointType == VG_JointType.FIXED)
                {
                    VG_Controller.RecoverObjectJoint(m_fixedObject);
                }
            }
        }
    }
}