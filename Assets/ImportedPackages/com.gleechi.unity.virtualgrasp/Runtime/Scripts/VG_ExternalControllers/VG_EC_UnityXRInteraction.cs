// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

//#define VG_USE_UNITYXRINTERACTION_HAND 

using System;
using System.Collections.Generic;
using UnityEngine;
#if VG_USE_UNITYXRINTERACTION_HAND
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace VirtualGrasp.Controllers
{
    /**
     * This is an external controller class that supports the action-based Unity XR Interaction toolkit controller as an external controller.
     * Please refer to https://docs.virtualgrasp.com/controllers.html for the definition of an external controller for VG, and to
     * https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.0/manual/index.html for the plugin itself.
     * 
     * The following requirements have to be met to be able to enable the #define VG_USE_UNITYINTERACTION_HAND above and use the controller:
     * - You have the "XR Plugin Management" package installed into your Unity project.
     * - You have the "XR Interaction Toolkit" package installed into your Unity project.
     * - The current setup does not seem to work for OpenXR as an XR plugin provider, you must have Oculus XR package installed into your Unity project.
     * - if you use Oculus, you use it through "OpenXR" (Oculus -> Tools -> OVR Utilities Plugin -> Set OVR to OpenXR)
     */

    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_vg_ec_unityxrinteraction." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_EC_UnityXRInteraction : VG_ExternalController
    {

#if VG_USE_UNITYXRINTERACTION_HAND
        private const string XRI_RESOURCE = "VG_XRI_Entries"; //"XRI Default Input Actions";
        private const string XRI_ACTIONMAP = "Player";
        private static VG_XRI_Entries m_xriEntries = null;
        static InputActionManager m_provider = null;
        private ActionBasedController m_controller = null;
#endif

        [Serializable]
        public class HandMapping : VG_BoneMapping
        {
            public override void Initialize(int avatarID, VG_HandSide side)
            {
                base.Initialize(avatarID, side);
                m_BoneToTransform = new Dictionary<int, Transform>()
            {
                { 0, Hand_WristRoot }
            };
            }
        }

        public VG_EC_UnityXRInteraction(int avatarID, VG_HandSide side, Transform origin)
        {
            m_avatarID = avatarID;
            m_handType = side;
            m_origin = origin;
            m_enabled = true;
            m_enablingDefine = "VG_USE_UNITYXRINTERACTION_HAND";

#if VG_USE_UNITYXRINTERACTION_HAND 
            m_enabled = true;
#else
            PrintNotEnabledError();
            m_enabled = false;
#endif
        }

        public void DisposeController()
        {
#if VG_USE_UNITYXRINTERACTION_HAND
            if (m_controller != null)
                GameObject.Destroy(m_controller.gameObject);
#endif
        }

        public new void Initialize()
        {
#if VG_USE_UNITYXRINTERACTION_HAND
            if (m_provider == null) m_provider = GameObject.FindObjectOfType<InputActionManager>();
            if (m_xriEntries == null)
                m_xriEntries = Resources.Load<VG_XRI_Entries>(XRI_RESOURCE);
            if (m_xriEntries == null)
            {
                Debug.LogError("Could not find " + XRI_RESOURCE);
                return;
            }
            else
            {
                m_xriEntries.Initialize();
                //Debug.Log(m_xriEntries);
            }

            InputActionAsset inputActionAsset = null;
            if (m_provider == null)
            {
                m_provider = GameObject.FindObjectOfType<VG_MainScript>().gameObject.AddComponent<InputActionManager>();
                inputActionAsset = Resources.Load<InputActionAsset>(m_xriEntries.Get(VG_XRI_Entries.XRI_Entry.RESOURCE));
                if (inputActionAsset != null)
                {
                    m_provider.actionAssets = new List<InputActionAsset>{ inputActionAsset };
                    m_provider.EnableInput();
                }
                else Debug.Log("Could not load input actions.");                
            }

            if (m_provider != null && m_provider.actionAssets.Count > 0 && m_provider.actionAssets[0] != null)
            {
                m_mapping = new HandMapping();
                base.Initialize();

                string handSide = (m_handType == VG_HandSide.LEFT) ? "Left" : "Right";
                
                // We put the ActionBasedController components on dummy GameObjects so they do not
                // affect the wrist transforms per se (this can't be disabled in the XRController it seems).
                GameObject controller = new(handSide + "Controller_ID" + m_avatarID);
                controller.transform.SetParent(m_provider.transform);
                m_controller = controller.AddComponent<ActionBasedController>();
                
                // Bind Position and Rotation Signals
                InputActionMap inputMap = m_provider.actionAssets[0].FindActionMap(XRI_ACTIONMAP);
                if (inputMap == null) Debug.LogError("Could not find map " + XRI_ACTIONMAP);
                else
                {
                    m_controller.enableInputTracking = true;
                    m_controller.updateTrackingType = XRBaseController.UpdateType.Update;

                    foreach (VG_XRI_Entries.XRI_Entry entry in new List<VG_XRI_Entries.XRI_Entry> { VG_XRI_Entries.XRI_Entry.POSITION, VG_XRI_Entries.XRI_Entry.ROTATION, VG_XRI_Entries.XRI_Entry.TRIGGER, VG_XRI_Entries.XRI_Entry.GRAB, VG_XRI_Entries.XRI_Entry.HAPTICS })
                    {
                        string actionName = m_xriEntries.Get(entry, handSide);
                        InputAction inputAction = inputMap.FindAction(actionName);
                        if (inputAction == null)
                        {
                            Debug.LogWarning("Could not find action " + actionName + " in " + XRI_ACTIONMAP + ".");
                            continue;
                        }
                        switch (entry)
                        {
                            case VG_XRI_Entries.XRI_Entry.POSITION:
                                m_controller.positionAction = new InputActionProperty(inputAction); break;
                            case VG_XRI_Entries.XRI_Entry.ROTATION:
                                m_controller.rotationAction = new InputActionProperty(inputAction); break;
                            case VG_XRI_Entries.XRI_Entry.TRIGGER:
                                m_controller.activateAction = new InputActionProperty(inputAction); break;
                            case VG_XRI_Entries.XRI_Entry.GRAB:
                                m_controller.selectAction = new InputActionProperty(inputAction); break;
                            case VG_XRI_Entries.XRI_Entry.HAPTICS:
                                m_controller.hapticDeviceAction = new InputActionProperty(inputAction); break;
                        }
                    }
                }
            }
        
            m_initialized = (m_provider != null && m_controller != null);
#endif
        }

        public override bool Compute()
        {
#if VG_USE_UNITYXRINTERACTION_HAND
            if (!m_enabled) return false;
            if (!m_initialized) { Initialize(); return false; }
            SetPose(0, Matrix4x4.TRS(m_controller.currentControllerState.position, m_controller.currentControllerState.rotation, Vector3.one));
#endif
            return true;
        }

        public override float GetGrabStrength()
        {
            float trigger = 0.0f;
#if VG_USE_UNITYXRINTERACTION_HAND
            if (!m_initialized) return 0.0f;
            switch (VG_Controller.GetGraspButton())
            {
                case VG_VrButton.TRIGGER:
                    trigger = m_controller.currentControllerState.activateInteractionState.value; break;
                case VG_VrButton.GRIP:
                    trigger = m_controller.currentControllerState.selectInteractionState.value; break;
                case VG_VrButton.GRIP_OR_TRIGGER:
                    trigger = Mathf.Max(m_controller.currentControllerState.activateInteractionState.value,
                                        m_controller.currentControllerState.selectInteractionState.value);
                    break;
            }
#endif
            return trigger;
        }

        public override Color GetConfidence()
        {
            return Color.yellow;
        }

        public override void HapticPulse(VG_HandStatus hand, float amplitude = 0.5F, float duration = 0.015F, int finger = 5)
        {
#if VG_USE_UNITYXRINTERACTION_HAND
            m_controller.SendHapticImpulse(amplitude, duration);
#endif
        }
    }
}