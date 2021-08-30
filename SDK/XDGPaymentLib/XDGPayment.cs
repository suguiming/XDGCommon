using System;
using System.Collections.Generic;
using XD.Intl.Common;

namespace XD.Intl.Payment
{
    public class XDGPayment
    {
        public static void PayWithProduct(string orderId, string productId, string roleId, string serverId, string ext,
            Action<XDGPaymentResult> callback)
        {
            XDGPaymentImpl.GetInstance().PayWithProduct(orderId, productId, roleId, serverId, ext, callback);
        }

        public static void PayWithWeb(string serverId, string roleId, Action<XDGError> callback)
        {
            XDGPaymentImpl.GetInstance().PayWithWeb(serverId, roleId, callback);
        }

        public static void QueryWithProductIds(string[] productIds, Action<XDGSkuDetailInfo> callback)
        {
            XDGPaymentImpl.GetInstance().QueryWithProductIds(productIds, callback);
        }

        public static void QueryRestoredPurchase(Action<List<XDGRestoredPurchase>> callback)
        {
            XDGPaymentImpl.GetInstance().QueryRestoredPurchase(callback);
        }

        public static void RestorePurchase(string purchaseToken, string orderId, string productId, string roleId, string serverId, string ext, Action<XDGOrderInfoWrapper> callback)
        {
            XDGPaymentImpl.GetInstance()
                .RestorePurchase(purchaseToken, orderId, productId, roleId, serverId, ext, callback);
        }

        public static void CheckRefundStatus()
        {
        }

        public static void CheckRefundStatusWithUI()
        {
        }
    }
}