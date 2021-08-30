using System;
using TapTap.Common;
using UnityEngine;
using XD.Intl.Common;
using TapTap.Bootstrap;

namespace XD.Intl.Account
{
    public class XDGAccountImpl{
        private XDGAccountImpl(){
              EngineBridge.GetInstance().Register(XDGUnityBridge.ACCOUNT_SERVICE_NAME, XDGUnityBridge.ACCOUNT_SERVICE_IMPL);
        }
        
        private readonly string XDG_ACCOUNT_SERVICE = "XDGLoginService"; //注意要和iOS本地的桥接文件名一样！ 
        private static volatile XDGAccountImpl _instance;
        private static readonly object Locker = new object();
        public static XDGAccountImpl GetInstance(){
            lock (Locker){
                if (_instance == null){
                    _instance = new XDGAccountImpl();
                }
            }
            return _instance;
        }
        
        public void Login(Action<XDGUser> callback, Action<XDGError> errorCallback){
            var command = new Command(XDG_ACCOUNT_SERVICE,"login",true, null);
                EngineBridge.GetInstance().CallHandler(command, (result => {
                    XDGTool.Log("Login 方法结果: " + result.ToJSON());
                    if (!XDGTool.checkResultSuccess(result)){
                        errorCallback(new XDGError(result.code, result.message));
                        return;
                    }

                    XDGUserWrapper userWrapper = new XDGUserWrapper(result.content);
                    if (userWrapper.error != null){
                        errorCallback(userWrapper.error);
                        return;
                    }
                    callback(userWrapper.user);
                }));
           
        }

        public void Logout(){
            var command = new Command(XDG_ACCOUNT_SERVICE, "logout", false, null);
            EngineBridge.GetInstance().CallHandler(command);
        }

        public void LoginSync(Action<bool> callback){
            var command = new Command(XDG_ACCOUNT_SERVICE,"loginSync",true, null);
                EngineBridge.GetInstance().CallHandler(command, (result => {
                    XDGTool.Log("LoginSync 方法结果: " + result.ToJSON());
                    if (!XDGTool.checkResultSuccess(result))
                    {
                        callback(false);
                        return;;
                    }
                    // TDSUser.BecomeWithSessionToken("anmlwi96s381m6ca7o7266pzf"); //bootstrap token激活
                    callback(true);
                }));
        }

        public void AddUserStatusChangeCallback(Action<int, string> callback){
            var command = new Command(XDG_ACCOUNT_SERVICE, "addUserStatusChangeCallback", true,
                    null);
                EngineBridge.GetInstance().CallHandler(command, (result) => {
                    XDGTool.Log("AddUserStatusChangeCallback 方法结果: " + result.ToJSON());
                    if (!XDGTool.checkResultSuccess(result)){
                        return;
                    }
                
                    XDGUserStatusChangeWrapper wrapper = new XDGUserStatusChangeWrapper(result.content);
                    callback(wrapper.code, wrapper.message);
                });
        }

        public void GetUser(Action<XDGUser> callback, Action<XDGError> errorCallback){
            var command = new Command(XDG_ACCOUNT_SERVICE, "getUser", true, null);
                EngineBridge.GetInstance().CallHandler(command, result => {
                    XDGTool.Log("GetUser 方法结果: " + result.ToJSON());
                    if (!XDGTool.checkResultSuccess(result)){
                        errorCallback(new XDGError(result.code, result.message));
                        return;
                    }
                
                    XDGUserWrapper userWrapper = new XDGUserWrapper(result.content);
                    if (userWrapper.error != null){
                        errorCallback(userWrapper.error);
                        return;
                    }
                    callback(userWrapper.user);
                });
        }

        public void OpenUserCenter(){
            var command = new Command(XDG_ACCOUNT_SERVICE, "openUserCenter", false, null);
                EngineBridge.GetInstance().CallHandler(command);
        }

        public void LoginByType(LoginType loginType, Action<XDGUser> callback, Action<XDGError> errorCallback){
            var command = new Command.Builder()
                    .Args("loginType", XDGUser.GetLoginTypeString(loginType)) //和app交互用的是字符串，如TapTap
                    .Service(XDG_ACCOUNT_SERVICE)
                    .Method("loginByType")
                    .Callback(true)
                    .CommandBuilder();
            
                EngineBridge.GetInstance().CallHandler(command, result => {
                    XDGTool.Log("LoginByType 方法结果: " + result.ToJSON());
                    if (!XDGTool.checkResultSuccess(result)){
                        errorCallback(new XDGError(result.code, result.message));
                        return;
                    }
                
                    XDGUserWrapper wrapper = new XDGUserWrapper(result.content);
                    if (wrapper.error != null){
                        errorCallback(wrapper.error);
                        return;
                    }
                    callback(wrapper.user);
                });
        }
    }
}