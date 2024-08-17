// Copyright (C) 2014-2024 Gleechi Technology AB. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace VirtualGrasp
{
    public class Build
    {
        [MenuItem("Tools/VirtualGrasp/Test Command Line Build Step Android")]
        static void CommandLineBuildAndroidEditor()
        {
            CommandLineBuildAndroid();
        }

        // Checks if only ARM64 is enabled for build to cancel build for any
        // other unsupported platform.
        class VerifyAndroidBuild : IPreprocessBuildWithReport
        {
            public int callbackOrder { get { return 1; } }

            public void OnPreprocessBuild(BuildReport report)
            {

                if (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64)
                {
                    throw new BuildFailedException("VirtualGrasp only supports ARM64 apk build. Please correct your Target Architecture(s) in Player Settings->Target Architectures.");
                }

                if (Time.fixedDeltaTime >= 0.0138f)
                {
                    throw new BuildFailedException("To avoid visual artifacts, VirtualGrasp has to run with higher than 72Hz, but your Fixed Timestep (Project Settings->Time) is set to " + (int)(1.0f / Time.fixedDeltaTime) + "Hz.\n" +
                        "For best performance, please update your Fixed Timestep setting.");
                }
            }
        }

        static string[] GetBuildScenes()
        {
            List<string> names = new();

            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;

                if (e.enabled)
                    names.Add(e.path);
            }
            return names.ToArray();
        }

        static string GetBuildPathAndroid()
        {
            return "build/" + Application.productName + ".apk";
        }

        static void CommandLineBuildAndroid()
        {
            Debug.Log("Command line build android version\n------------------\n------------------");

            string[] scenes = GetBuildScenes();
            if (scenes.Length == 0) return;

            string[] args = System.Environment.GetCommandLineArgs();
            string path = null;
            bool nextIsBuildPath = false;
            foreach (string arg in args)
            {
                if (nextIsBuildPath)
                {
                    path = arg;
                    nextIsBuildPath = false;
                }
                if (arg == "-buildPath") nextIsBuildPath = true;
            }

            if (path == null)
            {
                path = GetBuildPathAndroid();
                if (path == null) return;
            }

            Debug.Log(string.Format("Path: \"{0}\"", path));
            //for (int i = 0; i < scenes.Length; ++i)
            //    Debug.Log(string.Format("Scene[{0}]: \"{1}\"", i, scenes[i]));

            Debug.Log("Starting Android Build!");
            BuildPipeline.BuildPlayer(scenes, path, BuildTarget.Android, BuildOptions.None);
        }
    }
}
