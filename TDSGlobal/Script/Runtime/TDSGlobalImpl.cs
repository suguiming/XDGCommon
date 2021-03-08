using TDSCommon;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDSGlobal
{
    public class TDSGlobalImpl : ITDSGlobal
    {

        private volatile static ITDSGlobal sInstance;
        private static readonly object locker = new object();

        private TDSGlobalImpl()
        {
            Debug.Log("初始化TDSGlobalBridgeService");
            EngineBridge.GetInstance().Register(TDSGlobalBridgeName.SERVICE_CLZ, TDSGlobalBridgeName.SERVICE_IMPL);
            EngineBridge.GetInstance().Register(TDSGlobalBridgeName.IAP_SERVICE_CLZ, TDSGlobalBridgeName.IAP_SERVICE_IMPL);
            EngineBridge.GetInstance().Register(TDSGlobalBridgeName.LOGIN_SERVICE_CLZ, TDSGlobalBridgeName.LOGIN_SERVICE_IMPL);
        }


        public static ITDSGlobal GetInstance()
        {
            lock (locker)
            {
                if (sInstance == null)
                {
                    sInstance = new TDSGlobalImpl();
                }
            }
            return sInstance;
        }

        public void Init(Action<bool> callback)
        {
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "initTDSGlobalSDK", true, null);
            EngineBridge.GetInstance().CallHandler(command, (result) =>
            {
                Debug.Log("initSDK result:" + result.toJSON());

                if(!checkResultSuccess(result)){
                    return;
                }
                
                TDSGlobalInitWrapper initWrapper = new TDSGlobalInitWrapper(result.content);

                if(initWrapper!=null){
                    callback(initWrapper.success);
                }
            });
        }

        public void Login(Action<TDSGlobalUser> callback, Action<TDSGlobalError> errorCallback)
        {
            Command command = new Command(TDSGlobalBridgeName.LOGIN_SERVICE_NAME, "login", true, null);
            EngineBridge.GetInstance().CallHandler(command, (result) =>
            {   
                Debug.Log("login result:" + result.toJSON());

                if(!checkResultSuccess(result)){
                    return;
                }

                TDSGlobalUserWrapper userWrapper = new TDSGlobalUserWrapper(result.content);
                if(userWrapper.error!=null)
                {
                    errorCallback(userWrapper.error);
                    return;
                }            
                if(userWrapper.user!=null)
                {
                    callback(userWrapper.user);
                }
                    
            });
        }

        public void Logout()
        {
            Command command = new Command(TDSGlobalBridgeName.LOGIN_SERVICE_NAME, "logout", false, null);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void AddUserStatusChangeCallback(Action<int,string> callback)
        {
            Command command = new Command(TDSGlobalBridgeName.LOGIN_SERVICE_NAME, "addUserStatusChangeCallback", true,  null);
            EngineBridge.GetInstance().CallHandler(command, (result) =>
            {
                if(!checkResultSuccess(result)){
                    return;
                }
                
                TDSGlobalUserStatusChangeWrapper statusChangeWrapper = new TDSGlobalUserStatusChangeWrapper(result.content);
                callback(statusChangeWrapper.code,statusChangeWrapper.message);
            });
        }

        public void GetUser(Action<TDSGlobalUser> callback, Action<TDSGlobalError> errorCallback)
        {
            Command command = new Command(TDSGlobalBridgeName.LOGIN_SERVICE_NAME, "getUser", true, null);
            EngineBridge.GetInstance().CallHandler(command, (result) =>
            {
                Debug.Log("getUser result:" + result.toJSON());
                
                if(!checkResultSuccess(result)){
                    return;
                }
                TDSGlobalUserWrapper userWrapper = new TDSGlobalUserWrapper(result.content);
                if(userWrapper.user!=null){
                    callback(userWrapper.user);
                }
                else if(userWrapper.error!=null){
                    errorCallback(userWrapper.error);
                }                
            });
        }

        public void UserCenter()
        {
            Command command = new Command(TDSGlobalBridgeName.LOGIN_SERVICE_NAME, "userCenter", false, null);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void Share(int shareFlavors, string uri, string message, TDSGlobalShareCallback callback)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("url", uri);
            dic.Add("message", message);
            dic.Add("shareWithType", shareFlavors);
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "shareWithUriMessage", true, dic);
            EngineBridge.GetInstance().CallHandler(command, (result) =>
            {
                handlerShareCallback(result,callback);
            });
        }

        public void Share(int shareFlavors, string imagePath, TDSGlobalShareCallback callback)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("imagePath", imagePath);
            dic.Add("shareWithType", shareFlavors);
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "shareWithImage", true, dic);
            EngineBridge.GetInstance().CallHandler(command, (result) =>
            {
                handlerShareCallback(result,callback);
            });
        }

        public void SetLanguage(int languageType)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("setLanguage", languageType);
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "changeLanguageType", false, dic);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void PayWithProduct(string orderId, string productId, string roleId, string serverId, string ext,Action<TDSGlobalOrderInfo> callback,Action<TDSGlobalError> errorCallback)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("payWithOrderId", orderId);
            dic.Add("productId", productId);
            dic.Add("roleId", roleId);
            dic.Add("serverId", serverId);
            dic.Add("ext", ext);
            Command command = new Command(TDSGlobalBridgeName.IAP_SERVICE_NAME, "payWithProduct", true, dic);
            EngineBridge.GetInstance().CallHandler(command, (result) => { 
                handlerOrderInfoCallback(result,callback,errorCallback);
            });
        }

        public void PayWithWeb(string serverId, string roleId, Action<TDSGlobalError> callback)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("roleId", roleId);
            dic.Add("serverId", serverId);
            Command command = new Command(TDSGlobalBridgeName.IAP_SERVICE_NAME, "payWithWeb", true,  dic);
            EngineBridge.GetInstance().CallHandler(command, (result) => {
                if(!checkResultSuccess(result))
                {
                    return;
                } 
                TDSGlobalError error = new TDSGlobalError(result.content);
                callback(error);
             });
        }

        public void QueryWithProductIds(string[] productIds, Action<List<TDSGlobalSkuDetail>> callback,Action<TDSGlobalError> errorCallback)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("queryWithProductIds", productIds);
            Command command = new Command(TDSGlobalBridgeName.IAP_SERVICE_NAME, "querySKUWithProductIds", true, dic);
            EngineBridge.GetInstance().CallHandler(command, (result) => 
            { 
                if(!checkResultSuccess(result))
                {
                    return;
                }
                TDSGlobalSkuDetailWrapper wrapper = new TDSGlobalSkuDetailWrapper(result.content);
                if(wrapper==null)
                {
                    return;
                }
                if(wrapper.error!=null)
                {
                    errorCallback(wrapper.error);
                    return;
                }
                callback(wrapper.products);
            });
        }

        public void QueryRestoredPurchases(Action<List<TDSGlobalRestoredPurchases>> callback)
        {
            Command command = new Command(TDSGlobalBridgeName.IAP_SERVICE_NAME,"queryRestoredPurchases",true,null);
            EngineBridge.GetInstance().CallHandler(command, (result) => { 
                if(!checkResultSuccess(result))
                {
                    return;
                }
                TDSGlobalRestoredPurchasesWrapper wrapper = new TDSGlobalRestoredPurchasesWrapper(result.content);
                callback(wrapper.transactions);
            });
        }

        public void RestorePurchase(string tdsTransactionInfo, string orderId, string productId, string roleId, string serverId, string ext, Action<TDSGlobalOrderInfo> callback,Action<TDSGlobalError> errorCallback)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("restorePurchase", tdsTransactionInfo);
            dic.Add("orderId", orderId);
            dic.Add("roleId", roleId);
            dic.Add("serverId", serverId);
            dic.Add("ext", ext);
            dic.Add("productId", productId);
            Command command = new Command(TDSGlobalBridgeName.IAP_SERVICE_NAME, "queryRestoredPurchasesWithInfo", true, dic);
            EngineBridge.GetInstance().CallHandler(command, (result) => { 
                handlerOrderInfoCallback(result,callback,errorCallback);
            });
        }

        public void Report(string serverId, string roleId, string roleName)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("reportWithServerId", serverId);
            dic.Add("roleId", roleId);
            dic.Add("roleName", roleName);
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "report", false, dic);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void TrackUser(string serverId, string roleId, string roleName, int level)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("serverId", serverId);
            dic.Add("trackUser", roleId);
            dic.Add("roleName", roleName);
            dic.Add("level", level);
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "trackUser", false, dic);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void TrackEvent(string eventName)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("trackEvent", eventName);
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "trackEvent", false, dic);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void TrackAchievement()
        {
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "trackAchievement", false, null);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void EventCompletedTutorial()
        {
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "eventCompletedTutorial", false, null);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void EventCreateRole()
        {
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "eventCreateRole", false, null);
            EngineBridge.GetInstance().CallHandler(command);
        }
        
        public void GetVersionName(Action<string> callback)
        {
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME, "getTDSGlobalSDKVersion", true, null);
            EngineBridge.GetInstance().CallHandler(command, (result) =>
            {
                if(!checkResultSuccess(result)){
                    return;
                }
                callback(result.content);
            });
        }

        public void StoreReview()
        {
            Command command = new Command(TDSGlobalBridgeName.SERVICE_NAME,"storeReview",false,null);
            EngineBridge.GetInstance().CallHandler(command);
        }

        private bool checkResultSuccess(Result result){
            return result.code == Result.RESULT_SUCCESS && !string.IsNullOrEmpty(result.content);
        }

        private void handlerShareCallback(Result result,TDSGlobalShareCallback callback)
        {
            if(!checkResultSuccess(result))
            {
                return;
            }
            TDSGlobalShareWrapper shareWrapper = new TDSGlobalShareWrapper(result.content);
            Debug.Log("shareWrapper:" + shareWrapper.ToJSON());
            if(shareWrapper.cancel)
            {
                callback.ShareCancel();
                return;
            }
            if(shareWrapper.error!=null){
                if(!string.IsNullOrEmpty(shareWrapper.error.error_msg)){
                    callback.ShareError(shareWrapper.error.error_msg);
                    return;
                }
            }
            callback.ShareSuccess();
        }

        private void handlerOrderInfoCallback(Result result,Action<TDSGlobalOrderInfo> callback,Action<TDSGlobalError> errorCallback)
        {
            if(!checkResultSuccess(result))
            {
                return;
            }

            TDSGlobalOrderInfoWrapper infoWrapper = new TDSGlobalOrderInfoWrapper(result.content);
            if(infoWrapper == null)
            {
                return;
            }
            if(infoWrapper.error != null)
            {
                Debug.Log("OrderInfo Error Callback:" + infoWrapper.error);
                errorCallback(infoWrapper.error);
                return;
            }
            callback(infoWrapper.orderInfo);
        }

    }
}