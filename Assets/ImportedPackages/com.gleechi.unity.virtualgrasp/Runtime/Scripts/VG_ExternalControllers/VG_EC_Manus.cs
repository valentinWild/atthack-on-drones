// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

//#define VG_USE_MANUS_CONTROLLER 

using System;
using System.Collections.Generic;
using UnityEngine;
#if VG_USE_MANUS_CONTROLLER
using System.Linq;
using Manus;
using Manus.Skeletons;
using Manus.Utility;
#endif

namespace VirtualGrasp.Controllers
{
    /**
     * This is an external controller class that supports the Manus Glove controller as an external controller.
     * Please refer to https://docs.virtualgrasp.com/controllers.html for the definition of an external controller for VG.
     * 
     * The following requirements have to be met to be able to enable the #define VG_USE_MANUS_CONTROLLER above and use the controller:
     * - You have the corresponding Manus Core SDK (https://resources.manus-meta.com/downloads) installed on your computer.
     * - You have the Unity Plugin for Manus Core from https://resources.manus-meta.com/downloads imported into your Unity project.
     * - You have a Manus Pro License assigned to your SDK to use the Unity Plugin.
     */
    [LIBVIRTUALGRASP_UNITY_SCRIPT]
    [HelpURL("https://docs.virtualgrasp.com/unity_vg_ec_manus." + VG_Version.__VG_VERSION__ + ".html")]
    public class VG_EC_Manus : VG_ExternalController
    {
#if VG_USE_MANUS_CONTROLLER
        private Skeleton m_skeleton = null;
        private CoreSDK.SkeletonStream m_skeletonData;
        private int m_id = -1;
        private static uint MANUS_SKELETON_ID = 0;

        private void onSkeletonData(CoreSDK.SkeletonStream data)
        {
            m_skeletonData = data;
        }

        protected void IntFromManusBone(uint nodeId, out int boneId)
        {
            boneId = (int)nodeId;
        }

        static private void PrintNode(CoreSDK.SkeletonNode node)
        {
            Debug.Log("CNode " + node.ToString());
        }

        static private void PrintNode(Node node)
        {
            Debug.Log("Node " + node.id + " (" + node.name + "|" + node.nodeName + ")" +
                node.unityTransform.name + " (type: " + node.type + "; parent: " + node.parentID + ")"
                );
        }
        static private void PrintChain(Chain chain)
        {
            string str = "";
            foreach (uint id in chain.nodeIds)
                str += id + ",";
            Debug.Log("Chain " + chain.id + " (" + chain.name + "|" + chain.dataSide + ")"
                + " (type: " + chain.type + "; hand: " + chain.settings.finger.handChainId + "); meta " + chain.settings.finger.metacarpalBoneId
                + "; " + str);
        }
#endif

        [Serializable]
        public class HandMapping : VG_BoneMapping
        {
            public override void Initialize(int avatarID, VG_HandSide side)
            {
                base.Initialize(avatarID, side);
                m_BoneToTransform = new Dictionary<int, Transform>()
                {
#if VG_USE_MANUS_CONTROLLER
			    { 0, Hand_WristRoot },

                { 1, Hand_Thumb1 },
                { 2, Hand_Thumb2 },
                { 3, Hand_Thumb3 },
                { 4, null },

                { 5, null },
                { 6, Hand_Index1 },
                { 7, Hand_Index2 },
                { 8, Hand_Index3 },
                { 9, null },

                { 10, null },
                { 11, Hand_Middle1 },
                { 12, Hand_Middle2 },
                { 13, Hand_Middle3 },
                { 14, null },

                { 15, null },
                { 16, Hand_Ring1 },
                { 17, Hand_Ring2 },
                { 18, Hand_Ring3 },
                { 19, null },

                { 20, null },
                { 21, Hand_Pinky1 },
                { 22, Hand_Pinky2 },
                { 23, Hand_Pinky3 },
                { 24, null },                
#endif
                };

                m_BoneToParent = new Dictionary<int, int>()
                {
                };
#if VG_USE_MANUS_CONTROLLER
                m_BoneToParent[0] = -1;

                m_BoneToParent[1] = 0;
                m_BoneToParent[2] = 1;
                m_BoneToParent[3] = 2;
                m_BoneToParent[4] = 3;

                m_BoneToParent[5] = 0;
                m_BoneToParent[6] = 5;
                m_BoneToParent[7] = 6;
                m_BoneToParent[8] = 7;
                m_BoneToParent[9] = 8;

                m_BoneToParent[10] = 0;
                m_BoneToParent[11] = 10;
                m_BoneToParent[12] = 11;
                m_BoneToParent[13] = 12;
                m_BoneToParent[14] = 13;

                m_BoneToParent[15] = 0;
                m_BoneToParent[16] = 15;
                m_BoneToParent[17] = 16;
                m_BoneToParent[18] = 17;
                m_BoneToParent[19] = 18;

                m_BoneToParent[20] = 0;
                m_BoneToParent[21] = 20;
                m_BoneToParent[22] = 21;
                m_BoneToParent[23] = 22;
                m_BoneToParent[24] = 23;
#endif
            }

#if VG_USE_MANUS_CONTROLLER
            // Function to setup nodes for metacarpals and tips properly in Manus SDK.
            // (They are not considered in VG controllers.)
            private void FillUnassignedNodes(List<Node> nodes)
            {
                for (int nodeId = 0; nodeId < nodes.Count; nodeId++)
                {
                    Node node = nodes[nodeId];
                    if (node.unityTransform == null)
                    {
                        // metacarpals
                        if (new List<int>() { 5, 10, 15, 20 }.Contains(nodeId)) 
                        {
                            Node next_node = nodes[nodeId + 1];
                            node.unityTransform = next_node.unityTransform.parent;
                            node.nodeName = (node.unityTransform == null) ? "n/a" : node.unityTransform.name;
                            node.name = node.nodeName;
                        }
                        else // tips
                        {
                            Node prev_node = nodes[nodeId - 1];
                            node.unityTransform = prev_node.unityTransform.GetChild(0);
                            node.nodeName = (node.unityTransform == null) ? "n/a" : node.unityTransform.name;
                            node.name = node.nodeName;
                        }
                    }
                }
            }

            private bool ComputeFingerChainIds(out List<uint> chainIds)
            {
                chainIds = new List<uint>();
                foreach (Transform child in Hand_WristRoot.transform)
                {
                    List<Transform> children = child.GetComponentsInChildren<Transform>().ToList<Transform>();
                    if (children.Contains(Hand_Thumb1))
                        chainIds.Add(2);
                    else if (children.Contains(Hand_Index1))
                        chainIds.Add(3);
                    else if (children.Contains(Hand_Middle1))
                        chainIds.Add(4);
                    else if (children.Contains(Hand_Ring1))
                        chainIds.Add(5);
                    else if (children.Contains(Hand_Pinky1))
                        chainIds.Add(6);

                    // Check if we have properly ordered fingers.
                    int count = chainIds.Count;
                    if (count > 1 && chainIds[count - 1] < chainIds[count - 2])
                        return false;
                }

                return true;
            }

            public bool FillSkeletonFromMapping(Skeleton skeleton, VG_HandSide side)
            {
                skeleton.skeletonData = new SkeletonData();
                skeleton.skeletonData.type = CoreSDK.SkeletonType.Hand;
                skeleton.skeletonData.name = skeleton.name;
                skeleton.skeletonData.settings.targetType = CoreSDK.SkeletonTargetType.UserIndexData;
                skeleton.skeletonData.nodes.Clear();
                skeleton.skeletonData.chains.Clear();

                // Hand chain
                Chain chain1 = new Chain();
                chain1.id = 1;
                chain1.type = CoreSDK.ChainType.Hand;
                chain1.appliedDataType = chain1.type;
                chain1.dataSide = (side == VG_HandSide.LEFT) ? CoreSDK.Side.Left : CoreSDK.Side.Right;
                chain1.nodeIds.Add(0);
                chain1.settings.finger.handChainId = 0;
                chain1.settings.finger.metacarpalBoneId = 0;
                chain1.settings.hand.fingerChainIds = new[] { 2, 3, 4, 5, 6 };
                skeleton.skeletonData.chains.Add(chain1);
                
                // Add all nodes from VG mapping.
                foreach (KeyValuePair<int, Transform> boneTransform in m_BoneToTransform)
                {   
                    Node node = new Node();
                    node.unityTransform = boneTransform.Value;
                    node.nodeName = (node.unityTransform == null) ? "n/a" : node.unityTransform.name;
                    node.name = node.nodeName;
                    node.type = CoreSDK.NodeType.Joint;
                    node.id = (uint)boneTransform.Key;
                    node.parentID = m_BoneToParent.ContainsKey(boneTransform.Key) ?
                        (uint)m_BoneToParent[boneTransform.Key] : 0;
                    node.transform = new TransformValues();
                    node.transform.scale = Vector3.one;
                    skeleton.skeletonData.nodes.Add(node);
                }

                // Fill in nodes for bones that don't exist in VG skeleton
                FillUnassignedNodes(skeleton.skeletonData.nodes);

                // Finger chains (chain to bones)
                // Note: the chains need to be exactly ordered as in the hierarchy.

                if (!ComputeFingerChainIds(out List<uint> chainIds))
                {
                    Debug.LogError($"Please assure that the finger chains (children of {Hand_WristRoot.name}) are sorted as:" +
                        "(Thumb, Index, Middle, Ring, Pinky)", Hand_WristRoot);
                    return false;
                }
                
                foreach (uint chainId in chainIds)
                {
                    Chain chain = new Chain();
                    chain.name = "Finger " + chainId;
                    chain.id = chainId;
                    chain.type = (CoreSDK.ChainType)chainId + 3;
                    chain.appliedDataType = chain.type;
                    chain.dataSide = (side == VG_HandSide.LEFT) ? CoreSDK.Side.Left : CoreSDK.Side.Right;
                    chain.nodeIds = new List<uint>();
                    chain.settings.finger.handChainId = 1;
                    if (chainId == 2) // thumb
                    {
                        chain.settings.finger.metacarpalBoneId = -1;
                        chain.nodeIds = new List<uint> { 1, 2, 3, 4 };
                    }
                    else
                    {
                        chain.settings.finger.metacarpalBoneId = (int)(5 * (chainId - 2));
                        chain.nodeIds = new List<uint>();
                        for (int i = chain.settings.finger.metacarpalBoneId; i < chain.settings.finger.metacarpalBoneId + 5; i++)
                            chain.nodeIds.Add((uint)i);
                    }

                    skeleton.skeletonData.chains.Add(chain);
                }

                /*
                // Just for debugging
                foreach (Node node in skeleton.skeletonData.nodes)
                    PrintNode(node);
                foreach (Chain chain in skeleton.skeletonData.chains)
                    PrintChain(chain);
                */

                return true;
            }
#endif
        }

        public VG_EC_Manus(int avatarID, VG_HandSide side, Transform origin)
        {
            m_avatarID = avatarID;
            m_handType = side;
            m_origin = origin;
            m_enablingDefine = "VG_USE_MANUS_CONTROLLER";

#if VG_USE_MANUS_CONTROLLER
            m_enabled = true;
#else
            PrintNotEnabledError();
            m_enabled = false;
#endif
        }

        public new void Initialize()
        {
#if VG_USE_MANUS_CONTROLLER
            if (m_mapping == null)
            {
                m_mapping = new HandMapping();
                base.Initialize();
            }

            // Note: need to disable GO first so Skeleton constructor does not fail
            //       due to uninitialized data structures (FillSkeletonFromMapping).
            GameObject go = m_mapping.Hand_WristRoot.gameObject;
            bool oldActive = go.activeSelf;
            go.SetActive(false);
            if (!go.TryGetComponent<Skeleton>(out m_skeleton))
                m_skeleton = go.AddComponent<Skeleton>();

            // Note: paused needs to be modified to public so Manus API does not apply bone poses,
            //       but let's VG do this.
            m_skeleton.m_Paused = true;
            
            m_initialized = false;
            if (!(m_mapping as HandMapping).FillSkeletonFromMapping(m_skeleton, m_handType))
            {
                this.m_enabled = false;
                return;
            }

            try { go.SetActive(oldActive); }
            catch (Exception) { return; }

            // Note: ManusManager is static and managing the gloves
            m_skeleton.SetupMeshes();
            m_skeleton.SetupNodes();
            m_skeleton.SendSkeleton();
            m_skeleton.skeletonData.id = MANUS_SKELETON_ID++;
            m_id = (int)m_skeleton.skeletonData.id;
            ManusManager.communicationHub?.onSkeletonData.AddListener(this.onSkeletonData);
                
            m_initialized = true;
#endif
        }

        public override bool Compute()
        {
#if VG_USE_MANUS_CONTROLLER
        if (!m_enabled) return false;
        if (!m_initialized || m_mapping == null) { Initialize(); return false; }
        if (m_skeletonData.skeletons == null || m_skeletonData.skeletons.Count == 0) return false;
        
        foreach (CoreSDK.SkeletonNode node in m_skeletonData.skeletons[m_id].nodes)
        {
            IntFromManusBone(node.id, out int boneId);
            //Debug.Log("VG " + m_handType + ":" + m_id + " (" + boneId + "/" + GetNumBones() + "); MAN: " + node.id + "/" + m_skeletonData.skeletons[0].nodes.Length + "):\n" + node.transform.rotation.FromManus());

            if (boneId == 0)
            {
                SetPose(boneId, Matrix4x4.TRS(
                    node.transform.position.FromManus(),
                    node.transform.rotation.FromManus().normalized,
                    Vector3.one));
            }
            else
            {
                SetPose(boneId, m_poses[m_mapping.GetParent(boneId)] *
                    Matrix4x4.TRS(
                    node.transform.position.FromManus(),
                    node.transform.rotation.FromManus().normalized,
                    Vector3.one));
            }
        }

        return true;
#else
        return false;
#endif
        }

        public override float GetGrabStrength()
        {
#if VG_USE_MANUS_CONTROLLER
            // No grab strength from Manus API is available, so
            // let VG decide from full DOF.
            return -1.0f;
#else
            return 0.0f;
#endif
        }

        public override Color GetConfidence()
        {            
#if VG_USE_MANUS_CONTROLLER
            // No confidence value of the tracked data in Manus API.
            return Color.yellow;
#else
            return Color.yellow;
#endif
        }

        public override void HapticPulse(VG_HandStatus hand, float amplitude = 0.5F, float duration = 0.015F, int finger = 5)
        {
#if VG_USE_MANUS_CONTROLLER
            float[] amplitudes = new float[] { amplitude, amplitude, amplitude, amplitude, amplitude};
            ManusManager.communicationHub?.SendHapticDataForSkeleton(m_skeleton.skeletonData.id, m_handType == VG_HandSide.LEFT ? CoreSDK.Side.Left : CoreSDK.Side.Right, amplitudes);
#endif
        }
    }
}