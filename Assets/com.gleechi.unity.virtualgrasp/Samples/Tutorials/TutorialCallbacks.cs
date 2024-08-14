// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

#if UNITY_TUTORIALS_PACKAGE_INSTALLED
using UnityEngine;
using UnityEditor;
using Unity.Tutorials.Core.Editor;
using System.IO;
using UnityEngine.SceneManagement;
using VirtualGrasp.Controllers;

namespace VirtualGrasp.Tutorials
{
    [CreateAssetMenu(fileName = DefaultFileName, menuName = "Tutorials/" + DefaultFileName + " Instance")]
    public class TutorialCallbacks : ScriptableObject
    {
        /// <summary>
        /// The default file name used to create asset of this class type.
        /// </summary>
        public const string DefaultFileName = "TutorialCallbacks";

        /// <summary>
        /// Creates a TutorialCallbacks asset and shows it in the Project window.
        /// </summary>
        /// <param name="assetPath">
        /// A relative path to the project's root. If not provided, the Project window's currently active folder path is used.
        /// </param>
        /// <returns>The created asset</returns>
        public static ScriptableObject CreateAndShowAsset(string assetPath = null)
        {
            assetPath = assetPath ?? $"{TutorialEditorUtils.GetActiveFolderPath()}/{DefaultFileName}.asset";
            var asset = CreateInstance<TutorialCallbacks>();
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            EditorUtility.FocusProjectWindow(); // needed in order to make the selection of newly created asset to really work
            Selection.activeObject = asset;
            return asset;
        }

        public bool OneVGMainScriptExists()
        {
            return GameObject.FindObjectsOfType<VG_MainScript>().Length == 1;
        }

        public bool OneAvatarIsFingerControlled()
        {
            if (!OneVGMainScriptExists()) return false;
            VG_MainScript vg = GameObject.FindObjectsOfType<VG_MainScript>()[0];
            VG_ControllerProfile handProfile = Resources.Load<VG_ControllerProfile>("VG_ControllerProfiles/VG_CP_Oculus.OVRLib.HandTracking");
            foreach (VG_Avatar sensor in vg.m_avatars)
                if (sensor.m_primarySensorSetup.m_profile == handProfile)
                    return true;
            return false;
        }

        public bool IsOculusHandEnabled()
        {
            VG_EC_Oculus controller = new VG_EC_Oculus(0, VG_HandSide.LEFT, null);
            return controller.m_enabled;
        }

        public bool ActiveObjectHasVGMainScript()
        {
            return OneVGMainScriptExists() && Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<VG_MainScript>() != null;
        }
        public bool ActiveObjectHasVGArticulation()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<VG_Articulation>() != null;
        }

        public bool ActiveObjectMeshIsReadable()
        {
            if (!ActiveObjectHasVGArticulation()) return false;
            return Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh.isReadable;
        }

        public bool BakeReceived()
        {
            foreach (string f in Directory.GetFiles("Assets/VG_Grasps", "*.db"))
            {
                System.DateTime dbWrite = File.GetLastWriteTime(f);
                double diffNow = System.DateTime.Now.Subtract(dbWrite).TotalSeconds;
                if (diffNow < 60) return true;
            }

            return false;
        }

        public bool DebugFilesCreated()
        {
            if (!File.Exists("bake-job.zip")) return false;
            double diff = System.DateTime.Now.Subtract(Directory.GetLastWriteTime("bake-job.zip")).TotalSeconds;
            return (diff < 60);
        }

        public bool DebugSceneCreated()
        {
            if (!Directory.Exists("Assets/vg_tmp")) 
                return false;

            string file = "Assets/vg_tmp/" + SceneManager.GetActiveScene().name + ".scn";
            if (!File.Exists(file)) return false;

            double diff = System.DateTime.Now.Subtract(File.GetLastWriteTime(file)).TotalSeconds;
            return (diff < 60);
        }

        public bool GraspStudioResourcesAvailable()
        {
            VG_MainScript vg = VG_MainScript.Instance;

            if (vg = null)
                return false;
            if (vg.m_graspDB == null)
                return false;
            if (!File.Exists(AssetDatabase.GetAssetPath(vg.m_graspDB))) return false;
            if (!Directory.Exists("Assets/vg_tmp")) return false;
            return Directory.GetFiles("Assets/vg_tmp", "*.obj").Length > 0;
        }

        public bool IsGraspStudioScene()
        {
            //return GameObject.FindObjectsOfType<VG_GRas>().Length > 0;
            return false;
        }

        public bool SceneHasSkinnedMeshRenderer()
        {
            return GameObject.FindObjectsOfType<SkinnedMeshRenderer>().Length > 0;
        }

        public bool VGHasControlledAvatar()
        {
            if (!OneVGMainScriptExists()) return false;

            foreach (VG_Avatar avatar in VG_MainScript.GetEditorInstance().m_avatars)
            {
                if (!avatar.m_isRemote && !avatar.m_isReplay && avatar.m_skeletalMesh != null)
                    return true;

            }

            return false;
        }

        public bool VGHasControlledAvatarBySensor()
        {
            if (!VGHasControlledAvatar()) return false;
            return VG_MainScript.GetEditorInstance().m_avatars[0].m_primarySensorSetup.m_profile != null;
        }

        public void ShowProjectBrowser()
        {
            var projectBrowserType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            EditorWindow.GetWindow(projectBrowserType).Show();
        }
    }
}
#endif // #if UNITY_TUTORIALS_PACKAGE_INSTALLED