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
namespace TDSEditor
{
    
 public class TDSIOSPostBuildProcessor : MonoBehaviour
    {
#if UNITY_IOS
        // 添加标签，unity导出工程后自动执行该函数
        [PostProcessBuildAttribute(999)]
        /* 
            2020-11-20 Jiang Jiahao
            该脚本中参数为DEMO参数，项目组根据实际参数修改
            导出工程后核对配置或依赖是否正确，根据需要修改脚本
        */
        public static void OnPostprocessBuild(BuildTarget BuildTarget, string path)
        {   
            Debug.Log("build IOS");
            
            if (BuildTarget == BuildTarget.iOS)
            {   
                // 获得工程路径
                string projPath = PBXProject.GetPBXProjectPath(path);
                UnityEditor.iOS.Xcode.PBXProject proj = new PBXProject();
                proj.ReadFromString(File.ReadAllText(projPath));

                // 2019.3以上有多个target
#if UNITY_2019_3_OR_NEWER
                string unityFrameworkTarget = proj.GetUnityFrameworkTargetGuid();
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
                //获取bundleId
                var bundleId = proj.GetBuildPropertyForAnyConfig(target, "PRODUCT_BUNDLE_IDENTIFIER");
                // 编译配置
                proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
                proj.AddBuildProperty(unityFrameworkTarget, "OTHER_LDFLAGS", "-ObjC");

                // Swift编译选项
                proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO"); //bitcode  NO
                proj.SetBuildProperty(target,"ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES","YES");
                proj.SetBuildProperty(target, "SWIFT_VERSION", "5.0");
                proj.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
                proj.SetBuildProperty(unityFrameworkTarget, "ENABLE_BITCODE", "NO"); //bitcode  NO
                proj.SetBuildProperty(unityFrameworkTarget,"ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES","YES");
                proj.SetBuildProperty(unityFrameworkTarget, "SWIFT_VERSION", "5.0");
                proj.SetBuildProperty(unityFrameworkTarget, "CLANG_ENABLE_MODULES", "YES");
                
                // add extra framework(s)
                // 参数: 目标targetGUID, framework,是否required:fasle->required,true->optional
                proj.AddFrameworkToProject(unityFrameworkTarget, "CoreTelephony.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "QuartzCore.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "Security.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "WebKit.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "AdSupport.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "AssetsLibrary.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "AVKit.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "AuthenticationServices.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "LocalAuthentication.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "SystemConfiguration.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "Accelerate.framework", false);
                proj.AddFrameworkToProject(unityFrameworkTarget, "SafariServices.framework", false);
                // 动态库
                // AddFramework("TapFriends.framework", proj, target);
                Debug.Log("添加framework成功");

                // 添加 tbd
                // 参数: 目标targetGUID, tdbGUID
                proj.AddFileToBuild(unityFrameworkTarget, proj.AddFile("usr/lib/libc++.tbd", "libc++.tbd",PBXSourceTree.Sdk));
                
                Debug.Log("添加tbd成功");


                // 添加资源文件，注意文件路径
                var resourcePath = Path.Combine(path, "TDSGlobalResource");
                string parentFolder = Directory.GetParent(Application.dataPath).FullName;
                if (Directory.Exists(resourcePath))
                {
                    Directory.Delete(resourcePath,true);
                }

                Directory.CreateDirectory(resourcePath);

                string remotePackagePath = TDSFileHelper.FilterFile(parentFolder + "/Library/PackageCache/","com.tds.global");

                string localPacckagePath = TDSFileHelper.FilterFile(parentFolder,"TDSGlobal");
                
                string tdsResourcePath = remotePackagePath !=null? remotePackagePath + "/Plugins/IOS/Resource" : localPacckagePath + "/Plugins/IOS/Resource";

                Debug.Log("tdsGlobalResource:" + tdsResourcePath);

                if(Directory.Exists(tdsResourcePath)){
                    //使用UPM接入
                    CopyAndReplaceDirectory(tdsResourcePath, resourcePath);
                }
                // 复制资源文件夹到工程目录
                // 复制Assets的plist到工程目录
                File.Copy(parentFolder + "/Assets/Plugins/IOS/Resource/TDSGlobal-Info.plist",resourcePath + "/TDSGlobal-Info.plist");

                List<string> names = new List<string>();    
                names.Add("TDSGlobalSDKResources.bundle");
                names.Add("LineSDKResource.bundle");
                names.Add("GoogleSignIn.bundle");
                names.Add("TDSGlobal-Info.plist");
                foreach (var name in names)
                {
                    proj.AddFileToBuild(target, proj.AddFile(Path.Combine(resourcePath,name), Path.Combine(resourcePath,name), PBXSourceTree.Source));
                }

                Debug.Log("添加resource成功");
                Debug.Log("bundleId:" + bundleId);
                // rewrite to file  
                File.WriteAllText(projPath, proj.WriteToString());
                SetPlist(path,resourcePath + "/TDSGlobal-Info.plist",bundleId);
                SetScriptClass(path);
                Debug.Log("测试打包成功");
                return;
            }

        }

        // 添加动态库 注意路径
        public static void AddFramework(string coreFrameworkName, UnityEditor.iOS.Xcode.PBXProject proj, string target)
        {
            const string defaultLocationInProj = "Library/";
            string framework = Path.Combine(defaultLocationInProj, coreFrameworkName);
            string fileGuid = proj.AddFile(framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
            PBXProjectExtensions.AddFileToEmbedFrameworks(proj, target, fileGuid);
            proj.SetBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
        }

        // 修改Build版本号，使用时间 『年月日时分秒』
        public static string AddBuildNumber()
        {
            string buildNumber = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.Hour.ToString() +
            DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            return buildNumber;
        }

        public static void CopyAndReplaceDirectory(string srcPath, string dstPath)
        {
            if (Directory.Exists(dstPath))
                Directory.Delete(dstPath,true);
            if (File.Exists(dstPath))
                File.Delete(dstPath);

            Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath))
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

            foreach (var dir in Directory.GetDirectories(srcPath))
                CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
        }

        // 修改pilist
        private static void SetPlist(string pathToBuildProject,string infoPlistPath,string bundleId)
        {
            //添加info
            string _plistPath = pathToBuildProject + "/Info.plist";
            PlistDocument _plist = new PlistDocument();
            _plist.ReadFromString(File.ReadAllText(_plistPath));
            PlistElementDict _rootDic = _plist.root;

            List<string> items = new List<string>()
            {
                "tapsdk",
                "tapiosdk",
                "fbapi",
                "fbapi20130214",
                "fbapi20130410",
                "fbapi20130702",
                "fbapi20131010",
                "fbapi20131219",
                "fbapi20140410",
                "fbapi20140116",
                "fbapi20150313",
                "fbapi20150629",
                "fbapi20160328",
                "fb-messenger-share-api",
                "fbauth2",
                "fbshareextension",
                "lineauth2"
            };
            PlistElementArray _list = _rootDic.CreateArray("LSApplicationQueriesSchemes");
            for (int i = 0; i < items.Count; i++)
            {
                _list.AddString(items[i]);
            }
            
            Dictionary<string, object> dic = (Dictionary<string, object>)TDSEditor.Plist.readPlist(infoPlistPath);
            
            string facebookId = null;
            string taptapId = null;
            string googleId = null;

            foreach (var item in dic)
            {
                if(item.Key.Equals("facebook")){
                    Dictionary<string,object> facebookDic = (Dictionary<string,object>)item.Value;
                    foreach (var facebookItem in facebookDic)
                    {   
                        if(facebookItem.Key.Equals("app_id")){
                            facebookId = "fb" + (string)facebookItem.Value;
                        }
                    }
                }else if(item.Key.Equals("tapsdk")){
                    Dictionary<string,object> taptapDic = (Dictionary<string,object>) item.Value;
                    foreach (var taptapItem in taptapDic)
                    {
                        if(taptapItem.Key.Equals("client_id")){
                            taptapId = "tt" + (string) taptapItem.Value;
                        }
                    }
                }else if(item.Key.Equals("google")){
                    Dictionary<string,object> googleDic = (Dictionary<string,object>) item.Value;
                    foreach (var googleItem in googleDic)
                    {
                        if(googleItem.Key.Equals("REVERSED_CLIENT_ID")){
                            googleId = (string)googleItem.Value;
                        }
                    }
                }
            }

            //添加url
            PlistElementDict dict = _plist.root.AsDict();

            PlistElementArray array = dict.CreateArray("CFBundleURLTypes");
            PlistElementDict dict2 = array.AddDict();

            if(taptapId!=null)
            {
                dict2.SetString("CFBundleURLName", "TapTap");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString(taptapId);
            }

            if(googleId!=null)
            {
                dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "Google");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString(googleId);             
            }
            
            if(facebookId!=null)
            {
                dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "Facebook");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString(facebookId);
            }

            if(bundleId!=null)
            {
                dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "Line");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString("line3rdp." + bundleId);
            }
           
            File.WriteAllText(_plistPath, _plist.WriteToString());
            Debug.Log("修改添加info文件成功");
        }


        // 添加appdelegate处理
        private static void SetScriptClass(string pathToBuildProject)
        {
//             //插入代码
//             //读取UnityAppController.mm文件
            string unityAppControllerPath = pathToBuildProject + "/Classes/UnityAppController.mm";
            TDSEditor.TDSScriptStreamWriterHelper UnityAppController = new TDSEditor.TDSScriptStreamWriterHelper(unityAppControllerPath);
//             //在指定代码后面增加一行代码
            UnityAppController.WriteBelow(@"#import <OpenGLES/ES2/glext.h>", @"#import <TDSGlobalSDKCoreKit/TDSGlobalSDK.h>");
            UnityAppController.WriteBelow(@"[KeyboardDelegate Initialize];",@"[TDSGlobalSDK application:application didFinishLaunchingWithOptions:launchOptions];");
            UnityAppController.WriteBelow(@"AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);",@"[TDSGlobalSDK application:app openURL:url options:options];");
            UnityAppController.WriteBelow(@"NSURL* url = userActivity.webpageURL;",@"[TDSGlobalSDK application:application continueUserActivity:userActivity restorationHandler:restorationHandler];");
            UnityAppController.WriteBelow(@"handler(UIBackgroundFetchResultNoData);",@"[TDSGlobalSDK application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:completionHandler];");
            Debug.Log("修改代码成功");
        }
    }
}
    
#endif
#endif
