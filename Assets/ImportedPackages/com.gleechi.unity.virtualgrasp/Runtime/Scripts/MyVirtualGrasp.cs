// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using UnityEditor;
using System;

namespace VirtualGrasp.Scripts
{
    /**
     * MyVirtualGrasp is a customizable main tutorial component.
     *
     * MyVirtualGrasp inherits from VG_MainScript, which wraps the main communication functions of the VirtualGrasp API.
     * VG_MainScript inherits from Monobehavior so you can use this as a component to a GameObject in Unity.
     * All the API functions you want to use in your own scripts can be accessed through VG_Controller.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_component_myvirtualgrasp." + VG_Version.__VG_VERSION__ + ".html")]
    public class MyVirtualGrasp : VG_MainScript
    {
        override public void Awake()
        {
            base.Awake(); // note: Awake can delete this component if there already is one.
            if (this != null)
            {
                VG_Controller.Initialize();
            }
        }

        override public void Update()
        {
            base.Update();
        }

        override public void FixedUpdate()
        {
            base.FixedUpdate();
        }

        void OnApplicationQuit()
        {
            SaveState();
        }

        void OnApplicationPause()
        {
            // If linux save state
            var p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
                SaveState();
        }

        void SaveState()
        {
#if UNITY_EDITOR

            if (m_graspDB != null)
            {
                string graspDBPath = AssetDatabase.GetAssetPath(m_graspDB);
                VG_Controller.SaveState(graspDBPath);
                AssetDatabase.ImportAsset(graspDBPath);
            }
            else
            {
                VG_Controller.SaveState(null);
            }
#else
        VG_Controller.SaveState(null);
#endif
        }
    }
}