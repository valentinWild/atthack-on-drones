// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

//#define ENABLE_FAILING_TESTS

using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;

namespace VirtualGrasp.Tests
{
    using NAssert = NUnit.Framework.Assert;

    [TestFixture]
    public class InteractionLegacyTests
    {
        GameObject replaySetup;
        GameObject objectSetup;

        GameObject s_object;

        int replayAvatarID;

        [OneTimeSetUp]
        public void SetUpScene()
        {
            replaySetup = GameObject.Instantiate(Resources.Load("Testing/ReplayBaseLine")) as GameObject;
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/SingleObjectSetupLegacy")) as GameObject;
            GameObject.Find("Main Camera").transform.position = new Vector3(0, 3.3f, -1.5f);
            VG_Controller.GetReplayAvatarID(out replayAvatarID);
        }

        [SetUp]
        public void Setup()
        {
            s_object = GameObject.Find("Cube");
            s_object.transform.position = new Vector3(0.28f, 3.31f, -0.10f);
            s_object.transform.rotation = Quaternion.identity;
        }

        [UnityTest]
        public IEnumerator APickAndPlace()
        {
            NAssert.IsNotNull(s_object, "Could not find testable object in scene.");

            Transform obj = s_object.transform;
            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-pick-and-place");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS, "Invalid recording: " + recording.name);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitForSeconds(5.0f);

            VG_TestHelpers.PEqual(obj.position, new Vector3(0.681322932f, 3.37084985f, -0.42832908f));
            VG_TestHelpers.QEqual(obj.rotation, new Quaternion(-0.07935531f, 0.4471789f, 0.06912488f, 0.8882317f));
        }

        [UnityTest]
        public IEnumerator InteractPrismatic()
        {
            NAssert.IsNotNull(s_object, "Could not find testable object in scene.");

            Transform obj = s_object.transform;
            Assert.IsTrue(VG_Controller.ChangeObjectJoint(obj, VG_JointType.PRISMATIC, VG_MotionType.Limited, null, new Vector2(-1, 1)) == VG_ReturnCode.SUCCESS);
            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-pick-and-place");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitForSeconds(5.0f);

            VG_TestHelpers.PEqual(obj.position, new Vector3(0.28f, 3.31f, -0.34f));
            VG_TestHelpers.QEqual(obj.rotation, Quaternion.identity);
        }

        /*
        [UnityTest]
        public IEnumerator InteractPush()
        {
            NAssert.IsNotNull(Helpers.m_object, "Could not find testable object in scene.");

            VG_MainScript main = Transform.FindObjectOfType<VG_MainScript>();
            main.gameObject.AddComponent<VG_HintVisualizer>();

            Transform obj = Helpers.m_object.transform;
            //obj.position = new Vector3(10.28f, 3.31f, -0.10f);
            //obj.position = new Vector3(-0.10f, 3.53f, -0.10f);
            //obj.position = new Vector3(0.26f, 3.58f, 0.16f);
            obj.position = new Vector3(-0.095f, 3.58f, 0.358f);
            obj.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

            //obj.rotation = Quaternion.Euler(0, -90, 0);

            Assert.IsTrue(VG_Controller.LoadRecording("test-pick-and-place.sdb") == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay() == VG_ReturnCode.SUCCESS);

            yield return new WaitForSeconds(5.0f);

            Helpers.PEqual(obj.position, new Vector3(0.28f, 3.31f, -0.34f));
            Helpers.QEqual(obj.rotation, Quaternion.identity);
        }
        */

        [UnityTest]
        public IEnumerator InteractRevolute()
        {
            NAssert.IsNotNull(s_object, "Could not find testable object in scene.");

            Transform obj = s_object.transform;
            Assert.IsTrue(VG_Controller.ChangeObjectJoint(obj, VG_JointType.REVOLUTE, VG_MotionType.Limited, null, new Vector2(-180, 180)) == VG_ReturnCode.SUCCESS);
            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-pick-and-place");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitForSeconds(5.0f);

            VG_TestHelpers.PEqual(obj.position, new Vector3(0.2543455f, 3.281011f, -0.08325316f));
            VG_TestHelpers.QEqual(obj.rotation, new Quaternion(0, 0, -0.0849f, 0.9964f));
        }

        [UnityTest]
        public IEnumerator CheckSelectionPassed()
        {
            NAssert.IsNotNull(s_object, "Could not find testable object in scene.");

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-pick-and-place");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitUntil(() =>
                (VG_Controller.GetHand(replayAvatarID, VG_HandSide.RIGHT).m_selectedObject != null &&
                Time.realtimeSinceStartup >= 5.0f));
            Transform selectedObject = VG_Controller.GetHand(replayAvatarID, VG_HandSide.RIGHT).m_selectedObject;
            Assert.IsNotNull(selectedObject, "No object selected.");
            Assert.IsTrue(s_object.name == selectedObject.name, "Different object selected.");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            GameObject.Destroy(replaySetup);
            GameObject.Destroy(objectSetup);
        }
    }

    [TestFixture]
    public class InteractionReplayTests
    {
        GameObject replaySetup;
        GameObject avatarRoot;
        GameObject objectSetup;

        int replayAvatarID;

        [SetUp]
        public void Setup()
        {
            replaySetup = GameObject.Instantiate(Resources.Load("Testing/ReplayBaseLine")) as GameObject;
            avatarRoot = GameObject.Find("GleechiRig") as GameObject;
            avatarRoot.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            VG_Controller.GetReplayAvatarID(out replayAvatarID);
        }

        [UnityTest]
        public IEnumerator PushButton()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Move_empty_node_parent")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-push-button");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

#if ENABLE_FAILING_TESTS
        [UnityTest]
        public IEnumerator PushButtonWithPhysicalBase()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/button_indexpush_with_physical_base")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-button-indexpush-with-physical-base");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }
#endif

        [UnityTest]
        public IEnumerator PushShiftedButton()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Move_empty_node_parent")) as GameObject;

            GameObject.Find("base").transform.position = new Vector3(0, .2f, 0);

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-push-shifted-button");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }


        [UnityTest]
        public IEnumerator FloatingRevoluteVGDualHand()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_floating_revolute")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-floating-revolute");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator FloatingRevoluteVGDualHandFree()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_floating_revolute_free")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-floating-revolute-free");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator FloatingPrismVGDualHand()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_floating_prismatic")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-floating-prismatic");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator FloatingPrismVGDualHandFree()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_floating_prismatic_free")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-floating-prism-free");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator PrismVG()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_prismatic")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-prismatic");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator PrismVGFree()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_prismatic_free")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-prism-free");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator PrismAB()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_ab_prismatic")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-ab-prismatic");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

#if ENABLE_FAILING_TESTS
        [UnityTest]
        public IEnumerator RevoluteAB()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_ab_revolute")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-ab-revolute");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }
#endif

        [UnityTest]
        public IEnumerator FixedDualHandVG()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_revolute_multichain_3level")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-fixed-dual-hand");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator RevoluteDualHandVG()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_revolute_multichain_3level")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-revolute-dual-hand");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator PrismDualHandVG()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_prismatic")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-prismatic-dual-hand");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }


        [UnityTest]
        public IEnumerator RevoluteVGBounce()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_revolute_multichain_3level")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-revolute-bounce");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator RevoluteVGSnap()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_revolute_multichain_3level")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-revolute-snap");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }


        [UnityTest]
        public IEnumerator MultiRevoluteVGSnap()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_revolute_multichain_3level")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-multi-revolute-snap");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }


        [UnityTest]
        public IEnumerator RevolutePrismVGSnapBounce()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_multichain_3level")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-revolute-prismatic-snap-bounce");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }


        [UnityTest]
        public IEnumerator RevolutePrismVGDoubleBounce()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Off_pivot_vg_multichain_3level")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-revolute-prismatic-double-bounce");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

#if ENABLE_FAILING_TESTS
        [UnityTest]
        public IEnumerator StickyHandVG()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/StickyHandGraspPhysicalObject")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-sticky-hand");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }
#endif

        [UnityTest]
        public IEnumerator RevoluteFromRotationTranslation()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/Hammer_vg_revolute")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-vg-revolute-fromtr-rot");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

#if ENABLE_FAILING_TESTS
        [UnityTest]
        public IEnumerator ChainRootRigidbodyIskinematic()
        {
            // Note: since it is physical ensemble, it seems can not stably reproduce replay, each time run behave differently.
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/RuntimeInputTestChainRootRigidbodyIskinematic")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-chain-root-rigidbody-iskinematic");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator SwitchGraspObject()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/RuntimeInputTestSwitchGraspObject")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-switch-grasp-object");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator SetObjectJointState()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/RuntimeInputButtonTestSetObjectJointState")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-set-object-joint-state");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        /*        [UnityTest]
                public IEnumerator PhysicalHandCollision()
                {
                    GameObject baseTable = GameObject.Instantiate(Resources.Load("BaseTable")) as GameObject;
                    baseTable.transform.position = new Vector3(0, -.75f, 0);
                    objectSetup = GameObject.Instantiate(Resources.Load("Rigid_body_joint_wineglass_mug")) as GameObject;
                    objectSetup.transform.position =  Vector3.zero;
                    replaySetup = GameObject.Instantiate(Resources.Load("ReplayBaseLinePhysicalHand")) as GameObject;
                    avatarRoot = GameObject.Find("GleechiRig") as GameObject;
                    avatarRoot.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                    Assert.IsTrue(VG_Controller.LoadRecording("test-physical-hand.sdb") == VG_ReturnCode.SUCCESS);

                    Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

                    yield return new WaitWhile(() =>
                        VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                        VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

                    Assert.IsTrue(VG_Controller.IsReplaySuccess());

                    GameObject.Destroy(baseTable);
                }*/

        [UnityTest]
        public IEnumerator TestChangeObjectJoint()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/bottle_assemble_vgarticulation")) as GameObject;

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-change-object-joint");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }

        [UnityTest]
        public IEnumerator PlanarWithChangeJoint()
        {
            objectSetup = GameObject.Instantiate(Resources.Load("Testing/ChangeObjectJointPlanar")) as GameObject;

            objectSetup.transform.position = Vector3.zero;

            VG_TestChangeObjectJointPlanar script = objectSetup.transform.Find("grasp_button22").GetComponent<VG_TestChangeObjectJointPlanar>();

            VG_Recording recording = Resources.Load<VG_Recording>("Testing/Recordings/test-planar-switch");
            Assert.IsTrue(VG_Controller.LoadRecording(recording) == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(VG_Controller.StartReplay(replayAvatarID) == VG_ReturnCode.SUCCESS);

            for (int i = 0; i < 85; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            script.AttachToBoard();

            yield return new WaitWhile(() =>
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.LEFT) ||
                VG_Controller.IsReplaying(replayAvatarID, VG_HandSide.RIGHT));

            Assert.IsTrue(VG_Controller.IsReplaySuccess());
        }
#endif

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(objectSetup);
            GameObject.DestroyImmediate(replaySetup);
        }
    }


    [TestFixture]
    public class InteractionEngineTests
    {
        GameObject rootSetup;
        GameObject hammer;

        int replayAvatarID;

        [SetUp]
        public void SetUp()
        {

            rootSetup = GameObject.Instantiate(Resources.Load("Testing/ReplayBaseLine")) as GameObject;
            hammer = GameObject.Instantiate(Resources.Load("Testing/Hammer_vg_revolute")) as GameObject;
            VG_Controller.GetReplayAvatarID(out replayAvatarID);
        }

        [UnityTest]
        public IEnumerator GetObjectSelectionWeight()
        {
            VG_ReturnCode res = VG_Controller.GetObjectSelectionWeight(hammer.transform, out float weight);
            Assert.IsTrue(res == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(weight == 1);

            return null;
        }

        [UnityTest]
        public IEnumerator HideObjectSelectionWeight()
        {
            float new_weight = -1;
            bool eventReceived = false;

            VG_Controller.OnObjectSelectionWeightChanged.AddListener((transform, new_value) => {
                Assert.IsTrue(transform == hammer.transform);
                Assert.IsTrue(new_value == new_weight);
                eventReceived = true;
            });

            hammer.SetActive(false);

            VG_ReturnCode res = VG_Controller.GetObjectSelectionWeight(hammer.transform, out float weight);
            Assert.IsTrue(res == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(weight == new_weight);
            Assert.IsTrue(eventReceived);

            VG_Controller.OnObjectSelectionWeightChanged.RemoveAllListeners();

            return null;
        }

        [UnityTest]
        public IEnumerator RevealHiddenObjectSelectionWeight()
        {
            float new_weight = 1;
            bool eventReceived = false;

            hammer.SetActive(false);

            VG_Controller.OnObjectSelectionWeightChanged.AddListener((transform, new_value) => {
                Assert.IsTrue(transform == hammer.transform);
                Assert.IsTrue(new_value == new_weight);
                eventReceived = true;
            });

            hammer.SetActive(true);

            VG_ReturnCode res = VG_Controller.GetObjectSelectionWeight(hammer.transform, out float weight);
            Assert.IsTrue(res == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(weight == new_weight);
            Assert.IsTrue(eventReceived);

            VG_Controller.OnObjectSelectionWeightChanged.RemoveAllListeners();

            return null;
        }

        [UnityTest]
        public IEnumerator SetObjectSelectionWeight()
        {
            float new_weight = 0.5f;
            bool eventReceived = false;

            VG_Controller.OnObjectSelectionWeightChanged.AddListener((transform, new_value) => {
                Assert.IsTrue(transform == hammer.transform);
                Assert.IsTrue(new_value == new_weight);
                eventReceived = true;
            });

            VG_ReturnCode res = VG_Controller.SetObjectSelectionWeight(hammer.transform, new_weight);
            Assert.IsTrue(res == VG_ReturnCode.SUCCESS);
            VG_Controller.GetObjectSelectionWeight(hammer.transform, out float weight);
            Assert.IsTrue(weight == new_weight);
            Assert.IsTrue(eventReceived);

            VG_Controller.OnObjectSelectionWeightChanged.RemoveAllListeners();

            return null;
        }

        [UnityTest]
        public IEnumerator GetAvatarSpecificObjectSelectionWeight()
        {
            float new_weight = 0.5f;
            VG_Controller.SetAvatarSpecificObjectSelectionWeight(replayAvatarID, hammer.transform, new_weight);
            VG_ReturnCode res = VG_Controller.GetAvatarSpecificObjectSelectionWeight(replayAvatarID, hammer.transform, out float weight);
            Assert.IsTrue(res == VG_ReturnCode.SUCCESS);
            Assert.IsTrue(weight == new_weight);

            return null;
        }

        [UnityTest]
        public IEnumerator GetAvatarSpecificObjectSelectionWeightCheckError()
        {
            VG_ReturnCode res = VG_Controller.GetAvatarSpecificObjectSelectionWeight(replayAvatarID, hammer.transform, out float weight);
            Assert.IsTrue(res == VG_ReturnCode.ARGUMENT_ERROR);
            Assert.IsTrue(weight == 0);

            return null;
        }

        [UnityTest]
        public IEnumerator SetAvatarSpecificObjectSelectionWeight()
        {
            float new_weight = 0.5f;

            bool eventReceived = false;

            VG_Controller.OnAvatarSpecificObjectSelectionWeightChanged.AddListener((transform, avatarID, new_value) => {
                Assert.IsTrue(transform == hammer.transform);
                Assert.IsTrue(new_value == new_weight);
                eventReceived = true;
            });

            VG_ReturnCode res = VG_Controller.SetAvatarSpecificObjectSelectionWeight(replayAvatarID, hammer.transform, new_weight);
            Assert.IsTrue(res == VG_ReturnCode.SUCCESS);
            VG_Controller.GetAvatarSpecificObjectSelectionWeight(replayAvatarID, hammer.transform, out float weight);
            Assert.IsTrue(weight == new_weight);
            Assert.IsTrue(eventReceived);

            VG_Controller.OnAvatarSpecificObjectSelectionWeightChanged.RemoveAllListeners();

            return null;
        }

        [UnityTest]
        public IEnumerator ClearAvatarSpecificObjectSelectionWeight()
        {
            float object_weight = 0.7f;
            float avatar_specific_weight = 0.5f;

            bool eventReceived = false;

            VG_Controller.SetObjectSelectionWeight(hammer.transform, object_weight);
            VG_Controller.GetObjectSelectionWeight(hammer.transform, out float weight);
            Assert.IsTrue(weight == object_weight);

            VG_Controller.SetAvatarSpecificObjectSelectionWeight(replayAvatarID, hammer.transform, avatar_specific_weight);
            VG_Controller.GetAvatarSpecificObjectSelectionWeight(replayAvatarID, hammer.transform, out weight);
            Assert.IsTrue(weight == avatar_specific_weight);

            VG_Controller.OnAvatarSpecificObjectSelectionWeightChanged.AddListener((transform, avatarID, new_value) => {
                Assert.IsTrue(transform == hammer.transform);
                Assert.IsTrue(new_value == object_weight);
                eventReceived = true;
            });

            VG_ReturnCode res = VG_Controller.ClearAvatarSpecificObjectSelectionWeights(replayAvatarID);
            Assert.IsTrue(res == VG_ReturnCode.SUCCESS);

            Assert.IsTrue(eventReceived);

            res = VG_Controller.GetAvatarSpecificObjectSelectionWeight(replayAvatarID, hammer.transform, out weight);
            Assert.IsTrue(res == VG_ReturnCode.ARGUMENT_ERROR);

            VG_Controller.OnAvatarSpecificObjectSelectionWeightChanged.RemoveAllListeners();

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(hammer);
            GameObject.DestroyImmediate(rootSetup);

            //VG_TestHelpers.ReleaseSetup();
        }
    }
}
