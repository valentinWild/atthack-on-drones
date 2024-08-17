// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using VirtualGrasp.Scripts;


namespace VirtualGrasp.Tests
{
    using UAssert = UnityEngine.Assertions.Assert;

    public class VG_TestHelpers
    {
        static public GameObject s_object;

        static public void PrintDebugPos(Transform transform)
        {
            Debug.Log("new Vector3(" + transform.position.x + "f, " + transform.position.y + "f, " + transform.position.z + "f)");
        }

        static public void PrintDebugRot(Transform transform)
        {
            Debug.Log("new Vector3(" + transform.eulerAngles.x + "f, " + transform.eulerAngles.y + "f, " + transform.eulerAngles.z + "f)");
        }

        static public void PEqual(Vector3 u, Vector3 v, float tolerance = 0.05f)
        {
            UAssert.AreApproximatelyEqual(u.x, v.x, tolerance, string.Format("Object's X position not approximately equal  (t={0}; {1}).", tolerance, u.x));
            UAssert.AreApproximatelyEqual(u.y, v.y, tolerance, string.Format("Object's Y position not approximately equal  (t={0}; {1}).", tolerance, u.y));
            UAssert.AreApproximatelyEqual(u.z, v.z, tolerance, string.Format("Object's Z position not approximately equal  (t={0}; {1}).", tolerance, u.z));
            Debug.Log("Object position in tolerance " + (u - v).magnitude);
        }

        static public void QEqual(Quaternion q1, Quaternion q2, float tolerance = 2.0f)
        {
            float angle = Quaternion.Angle(q1, q2);
            UAssert.IsTrue(angle < tolerance, string.Format("Object's rotation not approximately equal (diff={0} > {1}, q1: {2} q2: {3}).", angle, tolerance, q1.ToString("F4"), q2.ToString("F4")));
            Debug.Log("Object rotation in tolerance " + angle);
        }

        static public void ResetObject()
        {
            s_object.transform.position = new Vector3(0.28f, 3.31f, -0.10f);
            s_object.transform.rotation = Quaternion.identity;
            s_object.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }

        static public void CreateSetup()
        {
            GameObject lightGameObject = new GameObject("Light");
            Light lightComp = lightGameObject.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.color = Color.white;
            lightGameObject.transform.position = new Vector3(0, 50, 0);

            GameObject cameraGameObject = new GameObject("Camera");
            Camera cameraComp = cameraGameObject.AddComponent<Camera>();
            cameraGameObject.tag = "MainCamera";
            cameraGameObject.transform.position = new Vector3(-1, 3, 0);
            cameraGameObject.transform.rotation = Quaternion.Euler(0, 90, 0);

            // Add the object
            s_object = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s_object.AddComponent<VG_Articulation>();
            ResetObject();

            // Add the hands
            GameObject hands = (GameObject)GameObject.Instantiate(Resources.Load("GleechiHands/GleechiRig"));

            // Add the prefab and configure it
            GameObject game = new GameObject("VirtualGrasp");
            VG_MainScript main = game.AddComponent<MyVirtualGrasp>();

            main.m_avatars.Add(new VG_Avatar());
            main.m_avatars[0].m_skeletalMesh = hands.GetComponentInChildren<SkinnedMeshRenderer>();
            main.m_avatars[0].m_isReplay = true;

            GameObject originGameObject = new GameObject("Origin");
            originGameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
            main.m_avatars[0].m_primarySensorSetup.m_origin = originGameObject.transform;
            main.m_avatars[0].m_primarySensorSetup.m_profile.m_sensor = VG_SensorType.EXTERNAL_CONTROLLER;

            VG_Controller.Initialize();
        }

        static public void ReleaseSetup()
        {
            VG_MainScript.DestroyImmediate(VG_MainScript.Instance);
        }
    }
}