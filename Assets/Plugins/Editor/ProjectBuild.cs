using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XD.Intl.Common;

class ProjectBuild : Editor
{
    //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }

        return names.ToArray();
    }

    private static void UpdateSetting(string key, string environmentKey, string defaultMacOSPath)
    {
        string defaultPath = defaultMacOSPath;
        if (!string.IsNullOrEmpty(defaultPath) && Directory.Exists(defaultPath))
        {
            XDGTool.Log("set:" + key + " path:" + defaultPath);
            EditorPrefs.SetString(key, defaultPath);
            return;
        }

        throw new DirectoryNotFoundException(string.Format("{0} {1} {2}", key, environmentKey, defaultPath));
    }

    static void BuildForAndroid()
    {
        string ExportPath = "";
        bool IsRND = false;
        string unityVersion = "";
        string upmVersion = "";
        foreach (string arg in Environment.GetCommandLineArgs())
        {
            XDGTool.Log("args:" + arg);
            if (arg.StartsWith("-EXPORT_PATH"))
            {
                ExportPath = arg.Split('=')[1].Trim('"');
                XDGTool.Log("ExportPath:" + ExportPath);
            }
            else if (arg.StartsWith("-IS_RND"))
            {
                IsRND = arg.Split('=')[1].Trim('"').Equals("true");
                XDGTool.Log("isRND:" + IsRND);
            }
            else if (arg.StartsWith("-UNITY_VERSION"))
            {
                unityVersion = arg.Split('=')[1].Trim('"');
            }
            else if (arg.StartsWith("-UPM_VERSION"))
            {
                upmVersion = arg.Split('=')[1].Trim('"');
            }
        }

        // 签名文件配置，若不配置，则使用Unity默认签名
        PlayerSettings.Android.keyaliasName = "wxlogin";
        PlayerSettings.Android.keyaliasPass = "111111";
        PlayerSettings.Android.keystoreName =
            Application.dataPath.Replace("/Assets", "") + "/sign_password_111111.keystore";
        PlayerSettings.Android.keystorePass = "111111";

        UpdateSetting("AndroidSdkRoot", "ANDROID_SDK",
            "/Applications/Unity/Hub/Editor/" + unityVersion + "/PlaybackEngines/AndroidPlayer/SDK");
        UpdateSetting("AndroidNdkRoot", "ANDROID_NDK",
            "/Applications/Unity/Hub/Editor/" + unityVersion + "/PlaybackEngines/AndroidPlayer/NDK");

        string path = (ExportPath + "/" + "TDSGlobalSDKUnityDemo_" + upmVersion + ".apk").Replace("//", "/");

        XDGTool.Log("path:" + path);
        try
        {
            BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
        }
        catch (Exception e)
        {
            XDGTool.LogError(e.Message);
        }
    }

    static void BuildForIOS()
    {
        string path = Application.dataPath.Replace("/Assets", "") + "/TDSGlobalSDKUnityDemo";

        AssetDatabase.Refresh();
        try
        {
            BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.iOS, BuildOptions.None);
        }
        catch (Exception m)
        {
            XDGTool.LogError(m.Message);
        }
    }
}