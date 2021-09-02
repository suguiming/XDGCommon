# XD-Intl Common

## 前提条件

使用 XDGCommon 前提是必须使用以下依赖库:

* [TapTap.Common](https://github.com/TapTap/TapCommon-Unity.git)
* [TapTap.Bootstrap](https://github.com/TapTap/TapBootstrap-Unity.git)
* [TapTap.Login](https://github.com/TapTap/TapLogin-Unity.git)
* [TapTap.TapDB](https://github.com/TapTap/TapDB-Unity.git)
* [LeanCloud](https://github.com/leancloud/csharp-sdk-upm)

## 命名空间

```c#
using XD.Intl.Common
```

## 接口描述

###初始化
```c#
XDGCommon.Init((success =>
{
    // 是否初始化成功
});
```
###设置SDK语言
```c#
XDGCommon.SetLanguage(LangType.ZH_CN);
```
####语言类型说明
```c#
public enum LangType
    {
        ZH_CN = 0,
        ZH_TW = 1,
        EN = 2,
        TH = 3,
        ID = 4,
        KR = 5,
        JP = 6,
        DE = 7,
        FR = 8,
        PT = 9,
        ES = 10,
        TR = 11,
        RU = 12,
    }
```
###  客服反馈
```c#
XDGCommon.Report(serverId,roleId,roleName);
```
###  分享
```c#
//分享回调
public interface XDGShareCallback
{
    void ShareSuccess();

    void ShareCancel();

    void ShareFailed(string error);
}

```

```c#
//分享URL和链接
XDGCommon.Share(shareFlavors,uri,message,XDGShareCallback);
//分享图片
XDGCommon.Share(shareFlavors,imagePath,XDGShareCallback);
```

###  日志上报

```c#
//用户信息上报
XDGCommon.TrackUser(roleId,roleName,serverId,level);
// 成就上报
XDGCommon.TrackAchievement()；
//创建角色
XDGCommon.EventCreateRole();
//完成新手指引
XDGCommon.EventCompletedTutorial();
```

###  跳转到应用商店

```c#
XDGCommon.StoreReview();
```

## 注意事项
###安卓

```c#

```
###iOS

确保XDG-Info.plist 拷贝到 Assets/Plugins/IOS目录中

```c#

//脚本拷贝 Assets/Plugins/IOS 下的资源文件并且添加到framework的依赖中
List<string> names = new List<string>();    
names.Add("XDGResources.bundle");
names.Add("LineSDKResource.bundle");
names.Add("GoogleSignIn.bundle");
names.Add("XDG-Info.plist");
foreach (var name in names)
{
    proj.AddFileToBuild(target, proj.AddFile(Path.Combine(resourcePath,name), Path.Combine(resourcePath,name), PBXSourceTree.Source));
}

//脚本自动添加plist
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

//自动添加 XDG-Info.plist 中的facebook、Google、TapTap等第三方SDK的ClientId
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
File.WriteAllText(_plistPath, _plist.WriteToString());

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
...