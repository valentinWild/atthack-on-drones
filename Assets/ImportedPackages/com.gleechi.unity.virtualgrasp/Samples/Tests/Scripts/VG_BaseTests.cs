// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace VirtualGrasp.Tests
{
    using NAssert = NUnit.Framework.Assert;

    [TestFixture]
    public class InitializationTests
    {
        private GameObject rootSetup;

        [OneTimeSetUp]
        public void SetUp()
        {
            //VG_TestHelpers.CreateSetup();

            rootSetup = GameObject.Instantiate(Resources.Load("Testing/TestBaseLine")) as GameObject;
        }

        [UnityTest]
        public IEnumerator ComponentsExist()
        {
            VG_MainScript main = Transform.FindObjectOfType<VG_MainScript>();
            Assert.IsNotNull(main, "No VG_MainScript found in scene.");
            yield return null;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            //VG_TestHelpers.ReleaseSetup();
            GameObject.Destroy(rootSetup);
        }
    }

    [TestFixture]
    public class RegistrationTests
    {
        private GameObject rootSetup;

        private SkinnedMeshRenderer InstantiateAvatar(string name)
        {
            GameObject avatar = GameObject.Instantiate(Resources.Load(name)) as GameObject;
            NAssert.IsNotNull(avatar, string.Format("Could not instantiate {0}.", name));
            SkinnedMeshRenderer renderer = avatar.GetComponentInChildren<SkinnedMeshRenderer>();
            NAssert.IsNotNull(renderer, "Instantiated object does not have a SkinnedMeshRenderer.");
            return renderer;
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            rootSetup = GameObject.Instantiate(Resources.Load("Testing/TestBaseLine")) as GameObject;
        }

        [UnityTest]
        public IEnumerator InitializePassed()
        {
            NAssert.IsTrue(VG_Controller.IsEnabled());
            yield return null;  //new WaitForSeconds(5.0f);
        }

        [UnityTest]
        public IEnumerator RegisterAvatarFromMainScriptPassed()
        {
            int numHands = 0;
            foreach (VG_HandStatus hand in VG_Controller.GetHands())
                numHands++;
            NAssert.Greater(numHands, 0, "Avatar registration failed, no avatars in scene.");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator RuntimeRegisterReplayAvatarPassed()
        {
            int id1, id2;
            SkinnedMeshRenderer renderer = InstantiateAvatar("GleechiHands/GleechiRig");
            NAssert.AreEqual(VG_Controller.RegisterReplayAvatar(renderer, out id1), VG_ReturnCode.SUCCESS, "RegisterReplayAvatar failed.");
            NAssert.AreEqual(VG_Controller.RegisterReplayAvatar(renderer, out id2), VG_ReturnCode.AVATAR_ALREADY_REGISTERED, "Registering already registered avatar should fail.");
            NAssert.AreEqual(id1, id2);
            NAssert.AreEqual(VG_Controller.UnRegisterAvatar(id1), VG_ReturnCode.SUCCESS, "UnRegisterAvatar failed.");
            yield return null;
        }

#if ENABLE_FAILING_TESTS
        [UnityTest]
        public IEnumerator RuntimeRegisterRemoteAvatarPassed()
        {
            int id1, id2;
            SkinnedMeshRenderer renderer = InstantiateAvatar("GleechiHands/GleechiRig");
            NAssert.AreEqual(VG_Controller.RegisterRemoteAvatar(renderer, out id1), VG_ReturnCode.SUCCESS, "RegisterRemoteAvatar failed.");
            NAssert.AreEqual(VG_Controller.RegisterRemoteAvatar(renderer, out id2), VG_ReturnCode.AVATAR_ALREADY_REGISTERED, "Registering already registered avatar should fail.");
            NAssert.AreEqual(id1, id2);
            yield return null;
        }
#endif

        [UnityTest]
        public IEnumerator RuntimeRegisterSensorAvatarPassed()
        {
            int id1, id2;
            VG_SensorSetup setup = new VG_SensorSetup();
            setup.m_profile = Resources.Load("VG_ControllerProfiles/VG_CP_Common.Mouse") as VG_ControllerProfile;
            NAssert.IsNotNull(setup.m_profile);
            SkinnedMeshRenderer renderer = InstantiateAvatar("GleechiHands/GleechiRig");
            NAssert.AreEqual(VG_Controller.RegisterSensorAvatar(renderer, out id1, setup), VG_ReturnCode.SUCCESS, "RegisterSensorAvatar failed.");
            NAssert.AreEqual(VG_Controller.RegisterSensorAvatar(renderer, out id2, setup), VG_ReturnCode.AVATAR_ALREADY_REGISTERED, "Registering already registered avatar should fail.");
            NAssert.AreEqual(id1, id2);
            NAssert.AreEqual(VG_Controller.UnRegisterAvatar(id1), VG_ReturnCode.SUCCESS, "UnRegisterAvatar failed.");
            yield return null;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            GameObject.Destroy(rootSetup);
            //VG_TestHelpers.ReleaseSetup();
        }
    }
}