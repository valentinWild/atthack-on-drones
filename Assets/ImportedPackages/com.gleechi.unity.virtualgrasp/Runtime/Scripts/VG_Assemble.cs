// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using VirtualGrasp;
using UnityEngine.Events;

namespace VirtualGrasp.Scripts
{
    /** 
     * VG_Assemble provides a tool to assemble / dissemble an object through VG_Articulation.
     * The MonoBehavior provides a tutorial on the VG API functions VG_Controller.ChangeObjectJoint and RecoverObjectJoint.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_component_vgassemble." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_Assemble : MonoBehaviour
    {
        public enum AxisType
        {
            [Tooltip("Not match any axis, i.e. no rotation match")]
            NoAxis = 0,
            [Tooltip("Match X axis")]
            XAxis = 1,
            [Tooltip("Match Y axis")]
            YAxis = 2,
            [Tooltip("Match Z axis")]
            ZAxis = 3,
            [Tooltip("Match X axis in both direction")]
            XAxisSymmetric = 4,
            [Tooltip("Match Y axis in both direction")]
            YAxisSymmetric = 5,
            [Tooltip("Match Z axis in both direction")]
            ZAxisSymmetric = 6
        }
        

        [Tooltip("If this object will be reparented to the parent of the desired pose transform when it is assembled.")]
        public bool m_assembleToParent = true;
        [Tooltip("The target pose(s) of the assembled object (or assemble anchor if provided).")]
        public List<Transform> m_desiredPoses = new List<Transform>();
        [Tooltip("Threshold to assemble when object (or assemble anchor) to desired pose is smaller than this value (m).")]
        public float m_assembleDistance = 0.05f;
        [Tooltip("Threshold to assemble when object (or assemble anchor) to desired rotation is smaller than this value (deg).")]
        public float m_assembleAngle = 45;
        [Tooltip("The axis of the Assemble Anchor if assigned (otherwise of the object) to be aligned for assembling to a desired rotation. If NoAxis then no match to target rotation.")]
        public AxisType m_assembleAxis = AxisType.YAxis;
        [Tooltip("The number of discrete steps across 360 deg around Assemble Axis to match the rotation. 0 means rotational symmetric.")]
        public int m_assembleSymmetrySteps = 0;
        [Tooltip("If provided will match this anchor to the desired pose, otherwise match this object.")]
        public Transform m_assembleAnchor = null;
        
        [Tooltip("Threshold to disassemble when object (or assemble anchor) distance to desired pose is bigger than this value (m).")]
        public float m_disassembleDistance = 0.25f;
        [Tooltip("If only allow dissasemble when object is at the zero joint state (most relevant for screw joint).")]
        public bool m_disassembleOnZeroState = false;

        [Tooltip("The VG Articulation of constrained (non-FLOATING) joint type to switch to when assembling an object. If null will use Fixed joint.")]
        public VG_Articulation m_assembleArticulation = null;

        [Tooltip("The VG Articulation of floating joint type to switch to when disassembling an object. Must be provided if original joint is non-Floating.")]
        public VG_Articulation m_disassembleArticulation = null;

        [Tooltip("If force the disassembled object to become physical. Only relevant if original joint is non-Floating.")]
        public bool m_forceDisassembledPhysical = false;

        [Tooltip("Event triggered when object is to be assembled (satisfying assembling criterior but before joint and parent change), return the matched target transform.")]
        public UnityEvent<Transform> OnBeforeAssembled = new UnityEvent<Transform>();
        [Tooltip("Event triggered when object is assembled, return the assembled target transform.")]
        public UnityEvent<Transform> OnAssembled = new UnityEvent<Transform>();
        [Tooltip("Event triggered when object is disassembled, return the target transform from which the object is disassembled.")]
        public UnityEvent<Transform> OnDisassembled = new UnityEvent<Transform>();

        private float m_timeAtDisassemble = 0.0F;
        private float m_assembleDelay = 2.0F;

        public Transform m_disassembleParent = null;
        private Transform m_desiredPose = null;

        void Start()
        {
            if (m_assembleArticulation == null)
                Debug.LogWarning("Assemble Articulation is not assigned, so assemble will use Fixed joint for " + transform.name, transform);
            else if (m_assembleArticulation.m_type == VG_JointType.FLOATING)
            {
                Debug.LogError(transform.name + "'s Assemble Articulation can not be FLOATING joint type.", transform);
                this.enabled = false;
            }

            if(VG_Controller.GetObjectJointType(transform, true, out VG_JointType jointType) == VG_ReturnCode.SUCCESS && jointType == VG_JointType.FLOATING 
                && m_disassembleParent == null)
                m_disassembleParent = transform.parent;

            if (VG_Controller.GetObjectJointType(transform, true, out VG_JointType originalJointType) == VG_ReturnCode.SUCCESS
                && originalJointType != VG_JointType.FLOATING)
            {
                if (m_disassembleArticulation == null)
                {
                    Debug.LogError(transform.name + "'s initial joint is constrained type " + originalJointType + ", Disassemble Articulation with FLOATING joint type needs to be assigned.", transform);
                    this.enabled = false;
                }
                else if (m_disassembleArticulation.m_type != VG_JointType.FLOATING)
                {
                    Debug.LogError(transform.name + "'s Disassemble Articulation has to be FLOATING joint type.", transform);
                    this.enabled = false;
                }

                // If originally is non-floating means disassemble parent should be original parent's parent
                if(m_disassembleParent == null)
                    m_disassembleParent = transform.parent.parent;
            }

            if (m_assembleAnchor == null)
                m_assembleAnchor = transform;
        }
        
        public void SetTargetTransformActive(bool active)
        {
            if(m_desiredPose != null)
                m_desiredPose.gameObject.SetActive(active);
        }
        void LateUpdate()
        {
            assembleByJointChange();
            disassembleByJointChange();
        }

        void assembleByJointChange()
        {
            Quaternion relRot = Quaternion.identity;
            if (!findTarget(ref relRot))
                return;

            VG_JointType jointType;
            if ((Time.timeSinceLevelLoad - m_timeAtDisassemble) > m_assembleDelay
               && VG_Controller.GetObjectJointType(transform, false, out jointType) == VG_ReturnCode.SUCCESS &&
               jointType == VG_JointType.FLOATING)
            {
                OnBeforeAssembled.Invoke(m_desiredPose);

                // Project object rotation axis to align to desired rotation axis.
                transform.SetPositionAndRotation(transform.position, relRot * transform.rotation);
                Vector3 offset = m_desiredPose.position - m_assembleAnchor.position;
                transform.SetPositionAndRotation(transform.position + offset, transform.rotation);

                if (m_assembleToParent)
                    transform.SetParent(m_desiredPose.parent);

                VG_ReturnCode ret = m_assembleArticulation ? VG_Controller.ChangeObjectJoint(transform, m_assembleArticulation) : VG_Controller.ChangeObjectJoint(transform, VG_JointType.FIXED);
                if (ret != VG_ReturnCode.SUCCESS)
                    Debug.LogError("Failed to ChangeObjectJoint() on " + transform.name + " with return code " + ret, transform);

                OnAssembled.Invoke(m_desiredPose);
            }
        }

        void disassembleByJointChange()
        {
            // When the object with simulated weight is in "heaving lifting" phase, disallow distance-based disassemble 
            if (VG_Controller.IsLiftingObject(transform))
                return;

            foreach (VG_HandStatus hand in VG_Controller.GetHands())
            {
                VG_JointType jointType;
                if (hand.m_selectedObject == transform && hand.IsHolding()
                    && VG_Controller.GetObjectJointType(transform, false, out jointType) == VG_ReturnCode.SUCCESS
                    && jointType != VG_JointType.FLOATING)
                {
                    getSensorControlledAnchorPose(hand, out Vector3 sensor_anchor_pos, out Quaternion sensor_anchor_rot);

                    if (isZeroState(jointType)
                        && (sensor_anchor_pos - m_assembleAnchor.position).magnitude > m_disassembleDistance
                        )
                    {
                        if (m_assembleToParent || m_disassembleParent == null)
                            transform.SetParent(transform.parent.parent);
                        else
                            transform.SetParent(m_disassembleParent);

                        VG_Controller.GetObjectJointType(transform, true, out VG_JointType originalJointType);
                        if (originalJointType == VG_JointType.FLOATING)
                        {
                            if (VG_Controller.RecoverObjectJoint(transform) != VG_ReturnCode.SUCCESS)
                            {
                                Debug.LogError("Failed to disassemble with RecoverObjectJoint() on " + transform.name);
                                return;
                            }

                        }
                        else if (m_disassembleArticulation != null)
                        {
                            if (VG_Controller.ChangeObjectJoint(transform, m_disassembleArticulation) != VG_ReturnCode.SUCCESS)
                            {
                                Debug.LogError("Failed to disassemble with ChangeObjectJoint() on " + transform.name);
                                return;
                            }
                            // When object originally has VG constrained joint type (non-Floating) it has to be a non-physical object,
                            // therefore disassemble will not recover its original physical property, so here we add rigid body
                            // if user choose to m_makeDisassembledPhysical.
                            if (m_forceDisassembledPhysical && !transform.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
                            {
                                rb = transform.gameObject.AddComponent<Rigidbody>();
                                rb.useGravity = true;
                                if (!transform.TryGetComponent<Collider>(out _))
                                {
                                    MeshCollider collider = transform.gameObject.AddComponent<MeshCollider>();
                                    collider.convex = true;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("Failed to disassemble with ChangeObjectJoint() since Disassemble Articulation on " + transform.name + " is not assigned.");
                            return;
                        }

                        m_timeAtDisassemble = Time.timeSinceLevelLoad;

                        OnDisassembled.Invoke(m_desiredPose);
                    }
                }
            }
        }

        bool isZeroState(VG_JointType jointType)
        {
            if (!m_disassembleOnZeroState)
                return true;

            // If object is of a joint type that has no relevant joint states, then no zero state control for disassemble so return true
            VG_Controller.GetObjectJointState(transform, out float jointState);
            if (jointType == VG_JointType.REVOLUTE || jointType == VG_JointType.PRISMATIC || jointType == VG_JointType.CONE)
                return jointState == 0.0F;
            else if (jointType == VG_JointType.PLANAR)
            {
                VG_Controller.GetObjectSecondaryJointState(transform, out float jointState2);
                return (jointState == 0 && jointState2 == 0);
            }
            else
                return true;
        }

        void getSensorControlledAnchorPose(VG_HandStatus hand, out Vector3 anchorPos, out Quaternion anchorRot)
        {
            // Compute relative pose of anchor to grasping hand pose
            Vector3 lp = Quaternion.Inverse(hand.m_hand.rotation) * (m_assembleAnchor.position - hand.m_hand.position);
            Quaternion lq = Quaternion.Inverse(hand.m_hand.rotation) * m_assembleAnchor.rotation;

            // Then evaluate anchor rotation corresponding to hand pose determined by sensor
            VG_Controller.GetSensorPose(hand.m_avatarID, hand.m_side, out Vector3 sensor_pos, out Quaternion sensor_rot);
            anchorPos = sensor_rot * lp + sensor_pos;
            anchorRot = sensor_rot * lq;
        }

        bool findTarget(ref Quaternion relRot)
        {
            // If object is already assembled, don't need to find target.
            if (VG_Controller.GetObjectJointType(transform, false, out VG_JointType jointType) == VG_ReturnCode.SUCCESS
                    && jointType != VG_JointType.FLOATING)
                return false;
            m_desiredPose = null;
            foreach (Transform pose in m_desiredPoses)
            {
                if (closeToTargetPose(m_assembleAnchor, pose, m_assembleAxis, m_assembleSymmetrySteps, ref relRot))
                {
                    m_desiredPose = pose;
                    return true;
                }
            }
            return false;
        }

        bool closeToTargetPose(Transform anchor, Transform target, AxisType assembleAxis, int angleSteps, ref Quaternion relRot)
        {
            return (target.position - anchor.position).magnitude < m_assembleDistance &&
                closeToTargetRotation(anchor, target, assembleAxis, angleSteps, ref relRot);
        }

        /// <summary>
        /// Check if rotationally close to target and compute relRot to rotate anchor to target
        /// </summary>
        /// <param name="anchor">Anchor whose rotation is to match to target</param>
        /// <param name="target">Target to map anchor rotation to</param>
        /// <param name="assembleAxis">Axis type to map rotation on a plane defined by this</param>
        /// <param name="angleSteps">number of steps of angles to define a set of evenly distributed axes on the plane defined by assembleAxis</param>
        /// <param name="relRot">Output, the relative rotation to apply to anchor to map to target</param>
        /// <returns></returns>
        bool closeToTargetRotation(Transform anchor, Transform target, AxisType assembleAxis, int angleSteps, ref Quaternion relRot)
        {
            relRot = Quaternion.identity;
            if (assembleAxis == AxisType.NoAxis)
                return true; 
            float angle = 0.0F;
            Quaternion relRot2 = Quaternion.identity;
            float angle2 = 0.0F;
            switch (assembleAxis)
            {
                case AxisType.XAxis:
                case AxisType.XAxisSymmetric:
                    computeRelativeRotationAngle(anchor.right, target.right, target.forward, (assembleAxis == AxisType.XAxisSymmetric)? 2 : 1, ref relRot, ref angle);
                    if(angleSteps >0)
                        computeRelativeRotationAngle((relRot * anchor.rotation) * Vector3.up, target.up, target.right, angleSteps, ref relRot2, ref angle2);
                    relRot = relRot2 * relRot;
                    break;
                case AxisType.YAxis:
                case AxisType.YAxisSymmetric:
                    computeRelativeRotationAngle(anchor.up, target.up, target.forward, (assembleAxis == AxisType.YAxisSymmetric) ? 2 : 1, ref relRot, ref angle);
                    if(angleSteps >0)
                        computeRelativeRotationAngle((relRot * anchor.rotation) * Vector3.right, target.right, target.up, angleSteps, ref relRot2, ref angle2);
                    relRot = relRot2 * relRot;
                    break;
                case AxisType.ZAxis:
                case AxisType.ZAxisSymmetric:
                    computeRelativeRotationAngle(anchor.forward, target.forward, target.right, (assembleAxis == AxisType.ZAxisSymmetric) ? 2 : 1, ref relRot, ref angle);
                    if(angleSteps >0)
                        computeRelativeRotationAngle((relRot * anchor.rotation) * Vector3.right, target.right, target.forward, angleSteps, ref relRot2, ref angle2);
                    relRot = relRot2 * relRot;
                    break;
                default:
                    break;
            }

            return (angle < m_assembleAngle && angle2 < m_assembleAngle);
        }

        /// <summary>
        /// Compute relative rotation and angle from anchor axis to a set of axis starting form target axis following a number of steps, will find smallest relRot / angle
        /// </summary>
        /// <param name="anchorAxis">The axis to rotate to one of the targetAxis</param>
        /// <param name="targetAxis">The target axis as starting axis to rotate to</param>
        /// <param name="axis">The axis orthorganal to targetAxis around which will rotate a step from starting targetAxis</param>
        /// <param name="angleSteps">The number of steps around 360 deg to rotate target axis, if 0 means rotational symmetric</param>
        /// <param name="relRot">Output, the minimum relative rotation to map anchor axis to target axis</param>
        /// <param name="angle">Output, angle corresponding to relRot</param>
        void computeRelativeRotationAngle(Vector3 anchorAxis, Vector3 targetAxis, Vector3 axis, int angleSteps, ref Quaternion relRot, ref float angle)
        {
            relRot = Quaternion.identity;
            angle = 360.0f;
            Quaternion rot = Quaternion.AngleAxis(360.0F / angleSteps, axis);
            float rel_angle = 0.0f;
            Quaternion rel_rot = Quaternion.identity;
            for (int i = 0; i < angleSteps; i++)
            {
                rel_rot = Quaternion.FromToRotation(anchorAxis, targetAxis);
                rel_rot.ToAngleAxis(out rel_angle, out _);
                if (rel_angle < angle)
                {
                    angle = rel_angle;
                    relRot = rel_rot;
                }
                targetAxis = rot * targetAxis;
            }

        }
    }
}
