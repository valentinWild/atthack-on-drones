// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;

namespace VirtualGrasp.Tests
{
    public class TestChangeObjectJointPlanar : MonoBehaviour
    {
        public Transform m_newParent = null;
        public Transform m_desiredPose = null;
        public VG_Articulation m_destArticulation = null;
        private VG_Articulation vg_articulation = null;

        // Start is called before the first frame update
        void Start()
        {
            transform.gameObject.TryGetComponent<VG_Articulation>(out vg_articulation);
        }

        // Update is called once per frame
        void Update()
        {
            VG_JointType jointType;
            VG_Controller.GetObjectJointType(this.transform, false, out jointType);
            if (Input.GetKeyDown(KeyCode.C) && jointType == VG_JointType.FLOATING)
            {
                AttachToBoard();
            }
            else if (Input.GetKeyDown(KeyCode.X) && jointType == VG_JointType.PLANAR)
            {
                MoveToCorner();
            }
        }

        public void AttachToBoard()
        {
            Debug.LogWarning("Change " + transform.name + " to target position as planar joint");
            this.transform.SetPositionAndRotation(m_desiredPose.position, m_desiredPose.rotation);

            if (m_newParent != null)
                this.transform.SetParent(m_newParent);

            VG_Controller.ChangeObjectJoint(transform, m_destArticulation);
            //Debug.LogError("GetSensorControlledAvatarID failed to get valid avatar_id!");
        }

        public void MoveToCorner()
        {
            Debug.Log("Move!!");

            Vector2 state = new Vector2(m_destArticulation.m_max, m_destArticulation.m_max2);
            Debug.LogWarning("Set " + transform.name + " planar joint state to " + state);
            VG_Controller.SetObjectJointState(transform, state.x, state.y);
        }
    }
}