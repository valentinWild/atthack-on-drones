// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualGrasp.Controllers
{

    [Serializable, CreateAssetMenu(fileName = "VG_XRI_Entries", menuName = "VirtualGrasp/VG_XRI_Entries")]
    public class VG_XRI_Entries : ScriptableObject
    {
        public enum XRI_Entry
        {
            RESOURCE,
            POSITION,
            ROTATION,
            TRIGGER,
            GRAB,
            HAPTICS
        }

        public void Initialize()
        {
            m_entries.Clear();
            TryAdd(XRI_Entry.RESOURCE, m_resourceName);
            TryAdd(XRI_Entry.POSITION, m_positionAction);
            TryAdd(XRI_Entry.ROTATION, m_rotationAction);
            TryAdd(XRI_Entry.TRIGGER, m_triggerAction);
            TryAdd(XRI_Entry.GRAB, m_grabAction);
            TryAdd(XRI_Entry.HAPTICS, m_hapticsAction);
        }

        public void TryAdd(XRI_Entry key, string value)
        {
            if (value != "")
                m_entries.Add(key, value);
        }

        public string Get(XRI_Entry key, string suffix = "")
        {
            if (!m_entries.TryGetValue(key, out string value))
                return "null";
            return value + suffix;
        }

        public override string ToString()
        {
            string str = "Valid Entries:\n";
            foreach (var entry in m_entries) { str += entry.ToString() + "\n"; }
            return str;
        }

        public string m_resourceName = "";
        public string m_positionAction = "";
        public string m_rotationAction = "";
        public string m_triggerAction = "";
        public string m_grabAction = "";
        public string m_hapticsAction = "";

        private Dictionary<XRI_Entry, string> m_entries = new();
    }
}