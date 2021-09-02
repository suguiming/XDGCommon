#if UNITY_EDITOR && UNITY_IOS
using System;
using System.Collections.Generic;
using System.IO;
using TapTap.Common.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using XD.Intl.Common;

namespace XDGEditor{
    public class TDSIOSPostBuildProcessor : MonoBehaviour{
        [PostProcessBuildAttribute(999)]
        public static void OnPostprocessBuild(BuildTarget BuildTarget, string path){
            XDGTool.Log("开始配置Xcode信息");

            if (BuildTarget == BuildTarget.iOS){
                // 获得工程路径
                var projPath = TapCommonCompile.GetProjPath(path);
                var proj = TapCommonCompile.ParseProjPath(projPath);
                var target = TapCommonCompile.GetUnityTarget(proj);
                var unityFrameworkTarget = TapCommonCompile.GetUnityFrameworkTarget(proj);
                
                if (target == null || unityFrameworkTarget == null){
                    XDGTool.LogError("target是空");
                    return;
                }
                
                proj.AddFrameworkToProject(unityFrameworkTarget, "AdServices.framework", true);
                proj.AddFrameworkToProject(unityFrameworkTarget, "iAd.framework", false);
                proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC -lc++ -lstdc++ -lz -weak_framework Accelerate");
                proj.AddBuildProperty(unityFrameworkTarget, "OTHER_LDFLAGS",
                    "-ObjC -lc++ -lstdc++ -lz -weak_framework Accelerate");

                // Swift编译选项
                proj.SetBuildProperty(target, "CODE_SIGN_IDENTITY",
                "iPhone Distribution: Shanghai Xiaoliu Technology Co., Ltd.");
                proj.SetBuildProperty(target, "PROVISIONING_PROFILE_SPECIFIER", "Everything 2020");
                proj.SetBuildProperty(target, "PROVISIONING_PROFILE", "6a542e15-b177-4e10-a884-31e7c51c4857");
                proj.SetBuildProperty(target, "CODE_SIGN_IDENTITY[sdk=iphoneos*]",
                "iPhone Distribution: Shanghai Xiaoliu Technology Co., Ltd.");
                
                proj.SetBuildProperty(unityFrameworkTarget, "CODE_SIGN_STYLE", "Manual");
                proj.SetBuildProperty(target, "CODE_SIGN_STYLE", "Manual");
                proj.SetBuildProperty(target, "DEVELOPMENT_TEAM", "NTC4BJ542G");
                proj.SetBuildProperty(target, "PRODUCT_BUNDLE_IDENTIFIER", "com.ios.dxyy");
                
                proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
                proj.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
                proj.SetBuildProperty(target, "SWIFT_VERSION", "5.0");
                proj.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");

                proj.SetBuildProperty(unityFrameworkTarget, "ENABLE_BITCODE", "NO");
                proj.SetBuildProperty(unityFrameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
                proj.SetBuildProperty(unityFrameworkTarget, "SWIFT_VERSION", "5.0");
                proj.SetBuildProperty(unityFrameworkTarget, "CLANG_ENABLE_MODULES", "YES");

                // 添加资源文件，注意文件路径
                var resourcePath = Path.Combine(path, "TDSGlobalResource");
                var parentFolder = Directory.GetParent(Application.dataPath)?.FullName;
                if (Directory.Exists(resourcePath)){
                    Directory.Delete(resourcePath, true);
                }

                Directory.CreateDirectory(resourcePath);

                //拷贝文件夹里的资源
                string tdsResourcePath = parentFolder + "/Assets/Plugins/iOS/Resource";
                if (Directory.Exists(tdsResourcePath)){
                    XDGFileHelper.CopyAndReplaceDirectory(tdsResourcePath, resourcePath);
                }

                //获取bundleId
                var bundleId = GetValueFromPlist(resourcePath + "/XDG-Info.plist", "bundle_id");
                List<string> names = new List<string>();
                names.Add("XDGResources.bundle");
                names.Add("LineSDKResource.bundle");
                names.Add("GoogleSignIn.bundle");
                names.Add("XDG-Info.plist");

                foreach (var name in names){
                    proj.AddFileToBuild(target,
                        proj.AddFile(Path.Combine(resourcePath, name), Path.Combine(resourcePath, name),
                            PBXSourceTree.Source));
                }

                File.WriteAllText(projPath, proj.WriteToString()); //保存

                //修改plist
                SetPlist(path, resourcePath + "/XDG-Info.plist", bundleId);
                //插入代码片段
                SetScriptClass(path);

                XDGTool.Log("Xcode信息配置成功");
            }
        }

        private static void SetPlist(string pathToBuildProject, string infoPlistPath, string bundleId){
            //添加info
            string _plistPath = pathToBuildProject + "/Info.plist";
            PlistDocument _plist = new PlistDocument();
            _plist.ReadFromString(File.ReadAllText(_plistPath));
            PlistElementDict _rootDic = _plist.root;

            List<string> items = new List<string>(){
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
                "fbauth",
                "fbshareextension",
                "lineauth2"
            };
            PlistElementArray _list = _rootDic.CreateArray("LSApplicationQueriesSchemes");
            for (int i = 0; i < items.Count; i++){
                _list.AddString(items[i]);
            }

            Dictionary<string, object> dic = (Dictionary<string, object>) Plist.readPlist(infoPlistPath);

            string facebookId = null;
            string taptapId = null;
            string googleId = null;
            string twitterId = null;

            foreach (var item in dic){
                if (item.Key.Equals("facebook")){
                    Dictionary<string, object> facebookDic = (Dictionary<string, object>) item.Value;
                    foreach (var facebookItem in facebookDic){
                        if (facebookItem.Key.Equals("app_id")){
                            facebookId = "fb" + (string) facebookItem.Value;
                        }
                    }
                }
                else if (item.Key.Equals("tapsdk")){
                    Dictionary<string, object> taptapDic = (Dictionary<string, object>) item.Value;
                    foreach (var taptapItem in taptapDic){
                        if (taptapItem.Key.Equals("client_id")){
                            taptapId = "tt" + (string) taptapItem.Value;
                        }
                    }
                }
                else if (item.Key.Equals("google")){
                    Dictionary<string, object> googleDic = (Dictionary<string, object>) item.Value;
                    foreach (var googleItem in googleDic){
                        if (googleItem.Key.Equals("REVERSED_CLIENT_ID")){
                            googleId = (string) googleItem.Value;
                        }
                    }
                }
                else if (item.Key.Equals("twitter")){
                    Dictionary<string, object> twitterDic = (Dictionary<string, object>) item.Value;
                    foreach (var twitterItem in twitterDic){
                        if (twitterItem.Key.Equals("consumer_key")){
                            twitterId = (string) twitterItem.Value;
                        }
                    }
                }
            }

            //添加url
            PlistElementDict dict = _plist.root.AsDict();
            PlistElementArray array = dict.CreateArray("CFBundleURLTypes");
            PlistElementDict dict2 = array.AddDict();

            if (taptapId != null){
                dict2.SetString("CFBundleURLName", "TapTap");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString(taptapId);
            }

            if (googleId != null){
                dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "Google");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString(googleId);
            }

            if (facebookId != null){
                dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "Facebook");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString(facebookId);
            }

            if (bundleId != null){
                dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "Line");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString("line3rdp." + bundleId);
            }

            if (twitterId != null){
                dict2 = array.AddDict();
                dict2.SetString("CFBundleURLName", "Twitter");
                PlistElementArray array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2 = dict2.CreateArray("CFBundleURLSchemes");
                array2.AddString("tdsg.twitter." + twitterId);
            }

            File.WriteAllText(_plistPath, _plist.WriteToString());
        }

        private static void SetScriptClass(string pathToBuildProject){
            //读取Xcode中 UnityAppController.mm文件
            string unityAppControllerPath = pathToBuildProject + "/Classes/UnityAppController.mm";
            XDGScriptStreamWriterHelper UnityAppController = new XDGScriptStreamWriterHelper(unityAppControllerPath);

            //在指定代码后面增加一行代码
            UnityAppController.WriteBelow(@"#import <OpenGLES/ES2/glext.h>", @"#import <XDGCommonSDK/XDGCommonSDK.h>");
            UnityAppController.WriteBelow(@"[KeyboardDelegate Initialize];",
                @"[XDGSDK application:application didFinishLaunchingWithOptions:launchOptions];");
            UnityAppController.WriteBelow(@"AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);",
                @"[XDGSDK application:app openURL:url options:options];");
            if (CheckoutUniversalLinkHolder(unityAppControllerPath, @"NSURL* url = userActivity.webpageURL;")){
                UnityAppController.WriteBelow(@"NSURL* url = userActivity.webpageURL;",
                    @"[XDGSDK application:application continueUserActivity:userActivity restorationHandler:restorationHandler];");
            }else{
                UnityAppController.WriteBelow(@"- (void)preStartUnity               {}",
                    @"-(BOOL) application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray<id<UIUserActivityRestoring>> * _Nullable))restorationHandler{[XDGSDK application:application continueUserActivity:userActivity restorationHandler:restorationHandler];return YES;}");
            }

            UnityAppController.WriteBelow(@"handler(UIBackgroundFetchResultNoData);",
                @"[XDGSDK application:application didReceiveRemoteNotification:userInfo fetchCompletionHandler:completionHandler];");
        }

        private static bool CheckoutUniversalLinkHolder(string filePath, string below){
            StreamReader streamReader = new StreamReader(filePath);
            string all = streamReader.ReadToEnd();
            streamReader.Close();
            int beginIndex = all.IndexOf(below, StringComparison.Ordinal);
            return beginIndex != -1;
        }

        private static string GetValueFromPlist(string infoPlistPath, string key){
            if (infoPlistPath == null){
                return null;
            }

            Dictionary<string, object> dic = (Dictionary<string, object>) Plist.readPlist(infoPlistPath);
            foreach (var item in dic){
                if (item.Key.Equals(key)){
                    return (string) item.Value;
                }
            }
            return null;
        }
    }
}

#endif