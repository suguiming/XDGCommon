using System;
using System.Collections.Generic;
using TapTap.Common;
using UnityEngine;
using XD.Intl.Common;

namespace XD.Intl.Payment
{
    public class XDGPaymentImpl : IPaymentAPI{
        private readonly string XDG_PAYMENT_SERVICE = "XDGPaymentService"; //注意要和iOS本地桥接类名一样

        public XDGPaymentImpl(){
            EngineBridge.GetInstance()
                .Register(XDGUnityBridge.PAYMENT_SERVICE_NAME, XDGUnityBridge.PAYMENT_SERVICE_IMPL);
        }

        private static volatile XDGPaymentImpl _instance;
        private static readonly object Locker = new object();

        public static XDGPaymentImpl GetInstance(){
            lock (Locker){
                if (_instance == null){
                    _instance = new XDGPaymentImpl();
                }
            }
            return _instance;
        }

        public void PayWithProduct(string orderId, string productId, string roleId, string serverId, string ext,
            Action<XDGPaymentResult> callback){
            var dic = new Dictionary<string, object>{
                    { "orderId", orderId },
                    { "productId", productId },
                    { "roleId", roleId },
                    { "serverId", serverId },
                    { "ext", ext }
                };

                var command = new Command.Builder()
                    .Service(XDG_PAYMENT_SERVICE)
                    .Method("payWithProduct")
                    .Args(dic)
                    .Callback(true)
                    .CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command,
                    (result) => {
                        XDGTool.Log("PayWithProduct 方法结果: " + result.ToJSON());
                        callback(new XDGPaymentResult(result.content));
                    });
        }

        public void PayWithWeb(string serverId, string roleId, Action<XDGError> callback){
            var dic = new Dictionary<string, object>{
                    { "serverId", serverId },
                    { "roleId", roleId }
                };
                var command = new Command.Builder()
                    .Service(XDG_PAYMENT_SERVICE)
                    .Method("payWithWeb")
                    .Args(dic)
                    .Callback(true)
                    .CommandBuilder();
                EngineBridge.GetInstance()
                    .CallHandler(command, result => {
                        XDGTool.Log("PayWithWeb 方法结果: " + result.ToJSON());
                        
                    });
        }

        public void QueryWithProductIds(string[] productIds, Action<XDGSkuDetailInfo> callback){
            var dic = new Dictionary<string, object>{
                    { "productIds", productIds }
                };
            
            var command = new Command.Builder()
                    .Service(XDG_PAYMENT_SERVICE)
                    .Method("querySKUWithProductIds")
                    .Args(dic)
                    .OnceTime(true)
                    .Callback(true)
                    .CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command, result => {
                    XDGTool.Log("QueryWithProductIds 方法结果: " + result.ToJSON());
                    callback(new XDGSkuDetailInfo(result.content));
                });
        }

        public void QueryRestoredPurchase(Action<List<XDGRestoredPurchase>> callback){
            var command = new Command.Builder()
                    .Service(XDG_PAYMENT_SERVICE)
                    .Method("queryRestoredPurchases")
                    .OnceTime(true)
                    .Callback(true)
                    .CommandBuilder();
                EngineBridge.GetInstance().CallHandler(command,
                    (result) => {
                        XDGTool.Log("QueryRestoredPurchase 方法结果: " + result.ToJSON());
                        
                        if (!XDGTool.checkResultSuccess(result)){
                            callback(null);
                            return;
                        }
                        // callback
                    });
        }

        public void RestorePurchase(string purchaseToken, string orderId, string productId, string roleId,
            string serverId, string ext, Action<XDGOrderInfoWrapper> callback){
            var dic = new Dictionary<string, object>{
                    { "purchaseToken", purchaseToken },
                    { "orderId", orderId },
                    { "roleId", roleId },
                    { "serverId", serverId },
                    { "ext", ext },
                    { "productId", productId }
                };
                var command = new Command.Builder()
                    .Service(XDG_PAYMENT_SERVICE)
                    .Method("restorePurchase")
                    .Args(dic)
                    .Callback(true)
                    .CommandBuilder();

                EngineBridge.GetInstance().CallHandler(command, (result) => {
                    XDGTool.Log("RestorePurchase 方法结果: " + result.ToJSON());
                    callback(new XDGOrderInfoWrapper(result.content));
                });
        }

        public void CheckRefundStatus(){
            
        }

        public void CheckRefundStatusWithUI(){
            
        }
    }
}