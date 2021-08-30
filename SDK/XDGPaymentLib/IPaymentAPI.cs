using System;
using System.Collections.Generic;
using XD.Intl.Common;

namespace XD.Intl.Payment
{
    public interface IPaymentAPI
    {
        void PayWithProduct(string orderId, string productId, string roleId, string serverId, string ext, Action<XDGPaymentResult> callback);
        void PayWithWeb(string serverId, string roleId, Action<XDGError> callback);
        void QueryWithProductIds(string[] productIds, Action<XDGSkuDetailInfo> callback);
        void QueryRestoredPurchase(Action<List<XDGRestoredPurchase>> callback);
        void RestorePurchase(string purchaseToken, string orderId, string productId, string roleId, string serverId, string ext, Action<XDGOrderInfoWrapper> callback);
        void CheckRefundStatus();
        void CheckRefundStatusWithUI();
    }
}