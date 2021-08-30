using System;
using System.Collections.Generic;
using TapTap.Bootstrap;
using TapTap.Common;
using UnityEngine;

namespace XD.Intl.Common
{
    public class XDGCommonImpl : ICommonAPI
    {
        private static readonly string COMMON_MODULE_UNITY_BRIDGE_NAME = "XDGCoreService";
        private static volatile XDGCommonImpl _instance;
        private static readonly object locker = new object();

        private XDGCommonImpl()
        {
            Debug.Log("===> Init XDG Bridge Service");
            EngineBridge.GetInstance()
                .Register(XDGUnityBridge.COMMON_SERVICE_NAME, XDGUnityBridge.COMMON_SERVICE_IMPL);
        }

        public static XDGCommonImpl GetInstance()
        {
            if (_instance != null) return _instance;
            lock (locker)
            {
                if (_instance == null)
                {
                    _instance = new XDGCommonImpl();
                }
            }

            return _instance;
        }

        public void Init(Action<bool> callback)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("initSDK")
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();
            
            EngineBridge.GetInstance().CallHandler(command, (result) =>
            {
                Debug.Log("===> Init XDG SDK result: " + result.ToJSON());
                if (!checkResultSuccess(result))
                {
                    callback(false);
                    return;
                }

                var wrapper = new XDGInitResultWrapper(result.content);
                if (wrapper.localConfigInfo.tapSdkConfig != null)
                {
                    var info = wrapper.localConfigInfo.tapSdkConfig;
                    var config = new TapConfig.Builder()
                        .ClientID(info.clientId) // 必须，开发者中心对应 Client ID
                        .ClientToken(info.clientSecret) // 必须，开发者中心对应 Client Token
                        .ServerURL(info.serverUrl) // 开发者中心 > 你的游戏 > 游戏服务 > 云服务 > 数据存储 > 服务设置 > 自定义域名 绑定域名
                        .RegionType(RegionType.IO) // 非必须，默认 CN 表示国内
                        .ConfigBuilder();
                    TapBootstrap.Init(config);
                }

                callback(wrapper.isSuccess);
            });
        }

        public void IsInitialized(Action<bool> callback)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("isInitialized")
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                Debug.Log("===> IsInitialized: " + result.ToJSON());
                if (!checkResultSuccess(result))
                {
                    callback(false);
                    return;
                }

                callback("true".Equals(result.content.ToLower()));
            });
        }

        public void SetLanguage(LangType langType)
        {
            Debug.Log("===> SetLanguage langType: " + langType);
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("setLanguage")
                .Args("langType", (int)langType)
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void Share(ShareFlavors shareFlavors, string imagePath, XDGShareCallback callback)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("shareWithImage")
                .Args("shareFlavors", (int)shareFlavors)
                .Args("imagePath", imagePath)
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                Debug.Log("===> share with type: " + shareFlavors + " result: " + result.ToJSON());
                if (!checkResultSuccess(result))
                {
                    callback.ShareFailed($"Share Failed:{result.message}");
                    return;
                }

                var shareWrapper = new XDGShareResultWrapper(result.content);
                if (shareWrapper.cancel)
                {
                    callback.ShareCancel();
                    return;
                }

                if (shareWrapper.error != null)
                {
                    if (!string.IsNullOrEmpty(shareWrapper.error.error_msg))
                    {
                        callback.ShareFailed(shareWrapper.error.error_msg);
                        return;
                    }
                }

                callback.ShareSuccess();
            });
        }

        public void Share(ShareFlavors shareFlavors, string uri, string message, XDGShareCallback callback)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("shareWithUriMessage")
                .Args("shareFlavors", (int)shareFlavors)
                .Args("uri", uri)
                .Args("message", message)
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                Debug.Log("===> share with type: " + shareFlavors + " result: " + result.ToJSON());
                if (!checkResultSuccess(result))
                {
                    callback.ShareFailed($"Share Failed:{result.message}");
                    return;
                }

                var shareWrapper = new XDGShareResultWrapper(result.content);
                if (shareWrapper.cancel)
                {
                    callback.ShareCancel();
                    return;
                }

                if (shareWrapper.error != null)
                {
                    if (!string.IsNullOrEmpty(shareWrapper.error.error_msg))
                    {
                        callback.ShareFailed(shareWrapper.error.error_msg);
                        return;
                    }
                }

                callback.ShareSuccess();
            });
        }

        public void Report(string serverId, string roleId, string roleName)
        {
            var argsDic = new Dictionary<string, object>
            {
                { "serverId", serverId },
                { "roleId", roleId },
                { "roleName", roleName }
            };
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("report")
                .Args(argsDic)
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void TrackRole(string serverId, string roleId, string roleName, int level)
        {
            var argsDic = new Dictionary<string, object>
            {
                { "serverId", serverId },
                { "roleId", roleId },
                { "roleName", roleName },
                { "level", level }
            };
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("trackRole")
                .Args(argsDic)
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void TrackUser(string userId)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("trackUser")
                .Args("userId", userId)
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void TrackEvent(string eventName)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("trackEvent")
                .Args("eventName", eventName)
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void EventCompletedTutorial()
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("eventCompletedTutorial")
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void EventCreateRole()
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("eventCreateRole")
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void GetVersionName(Action<string> callback)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("getSDKVersionName")
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                Debug.Log("===> GetVersionName: " + result.ToJSON());
                if (!checkResultSuccess(result))
                {
                    callback($"GetVersionName Failed:{result.message}");
                    return;
                }

                callback(result.content);
            });
        }

        public void TrackAchievement()
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("trackAchievement")
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void SetCurrentUserPushServiceEnable(bool enable)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("setCurrentUserPushServiceEnable")
                .Args("enable", enable)
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void IsCurrentUserPushServiceEnable(Action<bool> callback)
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("isCurrentUserPushServiceEnable")
                .OnceTime(true)
                .Callback(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command, result =>
            {
                Debug.Log("===> isCurrentUserPushServiceEnable: " + result.ToJSON());
                if (!checkResultSuccess(result))
                {
                    callback(false);
                    return;
                }

                callback(new XDGBooleanContentWrapper(result.content).isSuccess);
            });
        }

        public void StoreReview()
        {
            var command = new Command.Builder()
                .Service(COMMON_MODULE_UNITY_BRIDGE_NAME)
                .Method("storeReview")
                .OnceTime(true)
                .CommandBuilder();
            EngineBridge.GetInstance().CallHandler(command);
        }

        private bool checkResultSuccess(Result result)
        {
            return result.code == Result.RESULT_SUCCESS && !string.IsNullOrEmpty(result.content);
        }
    }
}