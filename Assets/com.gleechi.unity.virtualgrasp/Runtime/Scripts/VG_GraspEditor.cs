// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using UnityEngine.UI; // for Text UI
using System.Collections.Generic;

namespace VirtualGrasp.Scripts
{
    /**
     * VG_Highlighter exemplifies how you could enable runtime grasp editing into your application.
     * The MonoBehavior provides a tutorial on the VG API functions for some of the VG_Controller event functions, 
     * such as EditGrasp, GetInteractionTypeForObject and SetInteractionTypeForObject.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_component_vggraspannotator." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_GraspEditor : MonoBehaviour
    {
        /// An enum to describe a hand interaction type allowed for grasp editing (adding primary grasps)
        public enum VG_EditingInteractionType
        {
            TRIGGER_GRASP = 0,                   // Default, hand goes to object at grasp position
            PREVIEW_GRASP = 1,                   // Grasp is always previewed once object is selected, trigger will pick up the object
            JUMP_GRASP = 3,                      // Object jumps to hand when grasp is triggered
        }
        public Transform m_pad = null;
        public Transform m_addGraspButton = null;
        public Transform m_toggleInteractionButton = null;
        public Transform m_stepGraspButton = null;
        public Transform m_deleteGraspButton = null;
        public Transform m_deleteAllGraspsButton = null;
        [Tooltip("Interaction type used to add grasps.")]
        public VG_EditingInteractionType m_editingInteractionType = VG_EditingInteractionType.TRIGGER_GRASP;

        
        private bool m_sensorAvatarFound = false;
        private int m_sensorAvatarIDLeft = 0;
        private int m_sensorAvatarIDRight = 0;


        private List<ButtonContainer> m_containers = new List<ButtonContainer>();

        // Keep track of intersections
        private Dictionary<Transform, bool> m_intersections = new Dictionary<Transform, bool>();

        private delegate void EditFunction(VG_HandStatus hand);
        private delegate bool ValidateFunction(VG_HandStatus hand, out string text);
        private class ButtonContainer
        {
            public Transform m_root = null;
            public EditFunction m_editFunction = null;
            public ValidateFunction m_validateFunction = null;
            private Text m_text = null;
            private MeshRenderer m_renderer = null;
            private VG_Articulation m_articulation = null;
            public static HashSet<Transform> BUTTON_TRANSFORMS = new HashSet<Transform>();
            public ButtonContainer(Transform button, ValidateFunction validateFunc, EditFunction editFunc)
            {
                m_root = button;
                m_text = button.GetComponentInChildren<Text>();
                m_renderer = button.GetComponentInChildren<MeshRenderer>();
                m_articulation = button.GetComponentInChildren<VG_Articulation>();
                BUTTON_TRANSFORMS.Add(m_articulation.transform);
                m_editFunction = editFunc;
                m_validateFunction = validateFunc;
            }

            public bool Validate(VG_HandStatus hand)
            {
                bool valid = m_validateFunction(hand, out string text);
                if (m_text != null)
                {
                    m_text.text = text;
                    m_text.color = (valid && hand.IsHolding()) ? Color.black : Color.grey;
                }
                return valid;
            }

            public bool Trigger(VG_HandStatus hand, bool hasIntersection)
            {
                if (m_renderer == null || !hand.IsHolding())
                    return hasIntersection;

                VG_Controller.GetObjectJointState(m_root, out float state);
                float threshold = m_articulation.m_min + (m_articulation.m_max - m_articulation.m_min) * 0.2f;

                bool isIntersecting = state > threshold;
                if (hasIntersection != isIntersecting)
                {
                    VG_Controller.OnObjectCollided.Invoke(hand); // just to trigger haptics
                    if (isIntersecting) m_editFunction(hand);
                }

                return isIntersecting;
            }
        }

        private void Start()
        {
            m_containers.Add(new ButtonContainer(m_addGraspButton, ValidateAddGrasp, AddGrasp));
            m_containers.Add(new ButtonContainer(m_toggleInteractionButton, ValidateToggleInteraction, ToggleInteraction));
            m_containers.Add(new ButtonContainer(m_stepGraspButton, ValidateStepGrasp, StepGrasp));
            m_containers.Add(new ButtonContainer(m_deleteGraspButton, ValidateDeleteGrasp, DeleteGrasp));
            m_containers.Add(new ButtonContainer(m_deleteAllGraspsButton, ValidateDeleteAllGrasp, DeleteAllGrasp));

            if (VG_Controller.GetSensorControlledAvatarID(out m_sensorAvatarIDLeft, out m_sensorAvatarIDRight) == VG_ReturnCode.SUCCESS)
                m_sensorAvatarFound = true;
        }

        #region ContainerFunctions

        private bool IsValidObject(VG_HandStatus hand)
        {
            return hand != null && hand.m_selectedObject != null && hand.m_selectedObject.TryGetComponent<MeshRenderer>(out _);
        }

        private void AddGrasp(VG_HandStatus hand)
        {
            VG_Controller.EditGrasp(hand.m_avatarID, hand.m_side, VG_EditorAction.ADD_CURRENT);
        }

        private bool ValidateAddGrasp(VG_HandStatus hand, out string text)
        {
            if (!IsValidObject(hand))
            {
                text = "Add Grasp";
                return false;
            }

            if (VG_Controller.GetInteractionTypeForObject(hand.m_selectedObject) == VG_InteractionType.JUMP_PRIMARY_GRASP)
            {
                text = "No add grasp\ninteraction is JUMP_PRIMARY_GRASP!";
                return false;
            }

            text = "Add Grasp\n(" + hand.GetNumGraspsInDB() + ")";
            return true;
        }
        private void ToggleInteraction(VG_HandStatus hand)
        {
            VG_InteractionType current_type = VG_Controller.GetInteractionTypeForObject(hand.m_selectedObject);
            // Initially if object has non ideal grasp editing interaction type, first toggle allow switch to ideal editing interaction type
            if (current_type != (VG_InteractionType)m_editingInteractionType && current_type != VG_InteractionType.JUMP_PRIMARY_GRASP)
                VG_Controller.SetInteractionTypeForObject(hand.m_selectedObject, (VG_InteractionType)m_editingInteractionType);
            // Then later will always toggle between the ideal editing type and jump primary grasp type
            else
                VG_Controller.SetInteractionTypeForObject(hand.m_selectedObject, current_type != VG_InteractionType.JUMP_PRIMARY_GRASP ?
                    VG_InteractionType.JUMP_PRIMARY_GRASP : (VG_InteractionType)m_editingInteractionType);
        }

        private void StepGrasp(VG_HandStatus hand)
        {
            VG_Controller.TogglePrimaryGraspOnObject(hand.m_avatarID, hand.m_side, hand.m_selectedObject);
        }

        private bool ValidateToggleInteraction(VG_HandStatus hand, out string text)
        {
            if (!IsValidObject(hand))
            {
                text = "Toggle interaction";
                return false;
            }

            VG_InteractionType current_type = VG_Controller.GetInteractionTypeForObject(hand.m_selectedObject);
            if (current_type == (VG_InteractionType)m_editingInteractionType && hand.GetNumPrimaryGraspsInDB() == 0)
            {
                text = "No toggle interaction\nno grasp!";
                return false;
            }

            VG_InteractionType target_type = (current_type == (VG_InteractionType)m_editingInteractionType) ? VG_InteractionType.JUMP_PRIMARY_GRASP : (VG_InteractionType)m_editingInteractionType;
            text = "Toggle interaction To\n" + target_type + "";
            return true;
        }

        private bool ValidateStepGrasp(VG_HandStatus hand, out string text)
        {
            if (!IsValidObject(hand))
            {
                text = "Step grasp";
                return false;
            }

            if (hand.GetNumGraspsInDB() == 0)
            {
                text = "No step grasp\nno grasp!";
                return false;
            }

            VG_InteractionType currentInteractionType = VG_Controller.GetInteractionTypeForObject(hand.m_selectedObject);

            if (currentInteractionType != VG_InteractionType.JUMP_PRIMARY_GRASP)
            {
                text = "No step grasp\ninteraction is not JUMP_PRIMARY_GRASP!";
                return false;
            }

            text = "Step grasp";
            return true;
        }

        private bool ValidateDeleteGrasp(VG_HandStatus hand, out string text)
        {
            if (!IsValidObject(hand))
            {
                text = "Delete grasp";
                return false;
            }
            int numGrasps = hand.GetNumGraspsInDB();
            if (numGrasps == 0)
            {
                text = "No delete grasp";
                return false;
            }
            text = "Delete grasp\n(" + numGrasps + ")";
            return true;
        }

        private bool ValidateDeleteAllGrasp(VG_HandStatus hand, out string text)
        {
            if (!IsValidObject(hand))
            {
                text = "Delete all grasps";
                return false;
            }
            int numGrasps = hand.GetNumGraspsInDB();
            if (numGrasps == 0)
            {
                text = "No delete all grasps";
                return false;
            }
            text = "Delete all grasps\n(" + numGrasps + ")";
            return true;
        }

        private void DeleteGrasp(VG_HandStatus hand)
        {
            VG_Controller.EditGrasp(hand.m_avatarID, hand.m_side, VG_EditorAction.DELETE_CURRENT);
            if (hand.GetNumGraspsInDB() == 0)
                VG_Controller.SetInteractionTypeForObject(hand.m_selectedObject, (VG_InteractionType)m_editingInteractionType);
        }

        private void DeleteAllGrasp(VG_HandStatus hand)
        {
            VG_Controller.EditGrasp(hand.m_avatarID, hand.m_side, VG_EditorAction.DELETE_ALL_HAND_GRASPS);
            if (VG_Controller.GetNumGraspsInDB(hand.m_selectedObject, hand.m_avatarID, hand.m_side) == 0)
                VG_Controller.SetInteractionTypeForObject(hand.m_selectedObject, (VG_InteractionType)m_editingInteractionType);
            else
                Debug.LogError("After remove all hand grasp, num grasps still non zero");
        }

        #endregion // ContainerFunctions

        void Update()
        {
            if (!m_sensorAvatarFound)
            {
                if (VG_Controller.GetSensorControlledAvatarID(out m_sensorAvatarIDLeft, out m_sensorAvatarIDRight) == VG_ReturnCode.SUCCESS)
                    m_sensorAvatarFound = true;
            }

            if (!m_sensorAvatarFound)
                return;


            // Find selected object for left and right hand, but ignore if selected object is the pad or a button for this annotator
            Transform leftSelected = null;
            Transform rightSelected = null;
            VG_HandStatus status = VG_Controller.GetHand(m_sensorAvatarIDLeft, VG_HandSide.LEFT);
            if (status != null)
                leftSelected = ButtonContainer.BUTTON_TRANSFORMS.Contains(status.m_selectedObject) || status.m_selectedObject == m_pad ? null : status.m_selectedObject;
            status = VG_Controller.GetHand(m_sensorAvatarIDRight, VG_HandSide.RIGHT);
            if (status != null)
                rightSelected = ButtonContainer.BUTTON_TRANSFORMS.Contains(status.m_selectedObject) || status.m_selectedObject == m_pad ? null : status.m_selectedObject;

            // If no object selected or if both hand selected different objects, not allow annotating
            if ((leftSelected == null && rightSelected == null) ||
                (leftSelected != null && rightSelected != null && leftSelected != rightSelected))
            {
                foreach (ButtonContainer container in m_containers)
                    if (!container.Validate(null)) continue;
                return;
            }

            bool hasIntersection;
            VG_HandStatus hand = VG_Controller.GetHand((leftSelected != null) ? m_sensorAvatarIDLeft : m_sensorAvatarIDRight, 
                (leftSelected != null) ? VG_HandSide.LEFT : VG_HandSide.RIGHT);
            foreach (ButtonContainer container in m_containers)
            {
                if (!container.Validate(hand)) continue;
                // cache intersecting value in map so function only triggers in entering/exiting frame 
                hasIntersection = (m_intersections.ContainsKey(container.m_root)) ? m_intersections[container.m_root] : false;
                m_intersections[container.m_root] = container.Trigger(hand, hasIntersection);
            }
        }
    }
}