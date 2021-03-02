#if UNITY_EDITOR && UNITY_IOS
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif
using UnityEngine;
 public class TDSIOSPostBuildProcessor : MonoBehaviour
    {
#if UNITY_IOS
        // 添加标签，unity导出工程后自动执行该函数
        [PostProcessBuild]
        /* 
            2020-11-20 Jiang Jiahao
            该脚本中参数为DEMO参数，项目组根据实际参数修改
            导出工程后核对配置或依赖是否正确，根据需要修改脚本
        */
        public static void OnPostprocessBuild(BuildTarget BuildTarget, string path)
        {
            
            if (BuildTarget == BuildTarget.iOS)
            {   
                // 获得工程路径
                string projPath = PBXProject.GetPBXProjectPath(path);
                UnityEditor.iOS.Xcode.PBXProject proj = new PBXProject();
                proj.ReadFromString(File.ReadAllText(projPath));

                // 2019.3以上有多个target
#if UNITY_2019_3_OR_NEWER
                string unityFrameworkTarget = proj.TargetGuidByName("UnityFramework");
                string target = proj.GetUnityMainTargetGuid();
#else
                string unityFrameworkTarget = proj.TargetGuidByName("Unity-iPhone");
                string target = proj.TargetGuidByName("Unity-iPhone");
#endif
                if (target == null)
                {
                    Debug.Log("target is null ?");
                    return;
                }

                // Swift编译选项
                proj.SetBuildProperty(target, "CODE_SIGN_IDENTITY", "iPhone Developer: GU YUNZE (QNV4UFK7C2)");
                proj.SetBuildProperty(target, "PROVISIONING_PROFILE_SPECIFIER", "XDSDKDemo_Dev");
                proj.SetBuildProperty(target, "PROVISIONING_PROFILE", "e3b9cdf8-b425-4c00-aa8c-ba4d7982e662");
                proj.SetBuildProperty(target, "CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Developer: GU YUNZE (QNV4UFK7C2)");

                proj.SetBuildProperty(unityFrameworkTarget, "CODE_SIGN_STYLE", "Manual");
                proj.SetBuildProperty(target, "DEVELOPMENT_TEAM", "NTC4BJ542G");

                proj.SetBuildProperty(target, "PRODUCT_BUNDLE_IDENTIFIER", "com.xd.sdkdemo1");


                // rewrite to file  
                File.WriteAllText(projPath, proj.WriteToString());
                Debug.Log("Sign 成功");
                return;
            }
        }
        }
#endif
#endif
