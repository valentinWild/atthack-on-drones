// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


namespace VirtualGrasp.Scripts
{
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_component_vgruntimeregister." + VG_Version.__VG_VERSION__ + ".html")]

    public class VG_RunTimeRegister : MonoBehaviour
    {
        [Serializable]
        public class RigRegister
        {
            [Tooltip("The key to press to clone and register the rig from rigClone.")]
            public KeyCode m_registerRigKey = KeyCode.R;
            [Tooltip("The rig transform to test rig clone registering.")]
            public Transform m_rigClone = null;
            [Tooltip("Scale the rig uniformly.")]
            public float m_scale = 1.0f;
            [Tooltip("The new external controller sensor setup for any new avatar.")]
            public VG_SensorSetup m_sensorSetup = null;
        }

        [Serializable]
        public class PrefabRegister
        {
            [Tooltip("The key to press to clone and register the rig from rigClone.")]
            public KeyCode m_registerPrefabKey = KeyCode.P;
            [Tooltip("The prefab to test rig clone registering.")]
            public GameObject m_prefab = null;
            [Tooltip("Scale the prefab uniformly.")]
            public float m_scale = 1.0f;
            [Tooltip("The new external controller profile for the new avatar.")]
            public VG_ControllerProfile m_profile;
            [Tooltip("Override the origin of the controller profile. Try to find it by name in the prefab.")]
            public string m_originName;
        }

        [Tooltip("The rigs to register at certain key press events.")]
        private List<RigRegister> m_registerRigs = new();
        [Tooltip("The rigs to register at certain key press events.")]
        public List<PrefabRegister> m_prefabRigs = new();
        private Dictionary<int, GameObject> m_prefabClones = new();
        [Tooltip("Register the first prefab at start. In case we have no other in the scene.")]
        public bool m_registerFirstAvatarAtStart = true;
        [Tooltip("If an avatar is instantiated while another is there, remove the older one.")]
        public bool m_allowOnlyOneAvatar = false;
        private bool m_vrButtonWasPressed = false;

        [Header("Register Object")]
        [Tooltip("The key to press to clone and register the object from objectClone.")]
        public KeyCode m_registerObjectKey = KeyCode.O;
        [Tooltip("The object transform to test object clone registering.")]
        public Transform m_objectClone = null;
        [Header("Register Scene")]
        [Tooltip("The key to press to load another scene additively")]
        public KeyCode m_registerSceneKey = KeyCode.S;
        [Tooltip("The scene name to test scene loading. ActiveScene if left empty.")]
        public string m_sceneName = "";
        
        public void Start()
        {
            if (m_sceneName == "")
                m_sceneName = SceneManager.GetActiveScene().name;

            if (m_registerFirstAvatarAtStart && m_prefabRigs.Count > 0)
                RegisterAvatarAndController(m_prefabRigs[0]);
        }

        private void RegisterAvatarAndController(RigRegister rig)
        {
            if (rig.m_rigClone == null || rig.m_rigClone.GetComponentInChildren<SkinnedMeshRenderer>() == null)
                return;

            GameObject clone = GameObject.Instantiate(rig.m_rigClone.gameObject);
            clone.transform.localScale = rig.m_scale * Vector3.one;
            if (VG_Controller.RegisterSensorAvatar(clone.GetComponentInChildren<SkinnedMeshRenderer>(), out int id, rig.m_sensorSetup) != VG_ReturnCode.SUCCESS)
            {
                Destroy(clone);
                return;
            }
            clone.name = clone.name + "_ID" + id;
        }

        private void UnregisterControlledAvatar()
        {
            VG_Controller.GetSensorControlledAvatarID(out int avatarID);
            if (avatarID != -1)
            {
                VG_Controller.UnRegisterAvatar(avatarID);
                GameObject.Destroy(m_prefabClones[avatarID]);
                m_prefabClones.Remove(avatarID);
            }
        }

        private void RegisterAvatarAndController(PrefabRegister prefab)
        {
            if (m_allowOnlyOneAvatar)
                UnregisterControlledAvatar();

            GameObject clone = Instantiate(prefab.m_prefab);
            if (clone == null || clone.GetComponentInChildren<SkinnedMeshRenderer>() == null)
            {
                Debug.LogError("Prefab " + prefab.m_prefab.name + " could not be found or has no SkinnedMeshRenderer.", this);
                Destroy(clone);
                return;
            }
            clone.transform.localScale = prefab.m_scale * Vector3.one;
            VG_SensorSetup sensorSetup = new()
            {
                m_profile = prefab.m_profile,
                m_origin = clone.transform.Find(prefab.m_originName)
            };

            if (VG_Controller.RegisterSensorAvatar(clone.GetComponentInChildren<SkinnedMeshRenderer>(), out int id, sensorSetup) != VG_ReturnCode.SUCCESS)
            {
                Destroy(clone);
                return;
            }

            clone.name = prefab.m_prefab.name + "_ID" + id;
            m_prefabClones.Add(id, clone);
        }

        void Update()
        {
            // Clone an object and register it in runtime to the VG library.
            if (Input.GetKeyDown(m_registerObjectKey) && m_objectClone != null)
            {
                GameObject clone = GameObject.Instantiate(m_objectClone.gameObject, m_objectClone.parent, true);
                clone.name = m_objectClone.name + "_clone";
                clone.transform.position += 0.05f * Vector3.up;
                m_objectClone = clone.transform;
            }

            // Clone an avatar rig (existing in the scene) and register it in runtime to the VG library.
            foreach (RigRegister rig in m_registerRigs)
            {
                if (Input.GetKeyDown(rig.m_registerRigKey))
                    RegisterAvatarAndController(rig);
            }

            // Clone an avatar rig (existing in the scene) and register it in runtime to the VG library.
            InputDevice inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            foreach (PrefabRegister rig in m_prefabRigs)
            {
                if (Input.GetKeyDown(rig.m_registerPrefabKey))
                    RegisterAvatarAndController(rig);

                // Register specific avatars with XR X/Y buttons
                if (inputDevice.isValid)
                {
                    bool vrButtonIsPressed = false;
                    if (inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out vrButtonIsPressed))
                    {         
                        // Register the one marked with "1" as KeyCode with X
                        if (rig.m_registerPrefabKey == KeyCode.Alpha1 &&
                            vrButtonIsPressed && !m_vrButtonWasPressed)
                            RegisterAvatarAndController(rig);
                        m_vrButtonWasPressed = vrButtonIsPressed;
                    }
                    if (!m_vrButtonWasPressed && inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out vrButtonIsPressed))
                    {
                        // Register the one marked with "2" as KeyCode with Y
                        if (rig.m_registerPrefabKey == KeyCode.Alpha2 &&
                            vrButtonIsPressed && !m_vrButtonWasPressed)
                            RegisterAvatarAndController(rig);
                        m_vrButtonWasPressed = vrButtonIsPressed;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                VG_Controller.GetSensorControlledAvatarID(out int avatarID);
                VG_Controller.UnRegisterAvatar(avatarID);
            }
            
            if (Input.GetKeyDown(m_registerSceneKey))
            {
                SceneManager.LoadScene(m_sceneName, LoadSceneMode.Additive);
            }
        }
    }
}