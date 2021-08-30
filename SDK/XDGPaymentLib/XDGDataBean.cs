using System.Collections.Generic;
using TapTap.Common;
using UnityEngine;
using XD.Intl.Common;

namespace XD.Intl.Payment
{
    public class XDGSkuDetailInfo
    {
        public XDGError xdgError;

        public List<SkuDetailBean> skuDetailList;

        public XDGSkuDetailInfo(string jsonStr)
        {
            var dic = Json.Deserialize(jsonStr) as Dictionary<string, object>;
            var errorDic = SafeDictionary.GetValue<Dictionary<string, object>>(dic, "error");
            if (errorDic != null)
            {
                xdgError = new XDGError(errorDic);
            }

            var list = SafeDictionary.GetValue<List<object>>(dic, "products");
            if (list == null) return;
            skuDetailList = new List<SkuDetailBean>();
            foreach (var skuDetail in list)
            {
                var innerDic = skuDetail as Dictionary<string, object>;
                var skuDetailBean = new SkuDetailBean(innerDic);
                skuDetailList.Add(skuDetailBean);
            }
        }
    }

    public class SkuDetailBean
    {
        public string description;

        public string freeTrialPeriod;

        public string iconUrl;

        public string introductoryPrice;

        public long introductoryPriceAmountMicros;

        public int introductoryPriceCycles;

        public string originJson;

        public string originPrice;

        public long originPriceAmountMicros;

        public string price;

        public long priceAmountMicros;

        public string priceCurrencyCode;

        public string productId;

        public string subscriptionPeriod;

        public string title;

        public string type;

        public SkuDetailBean(Dictionary<string, object> dic)
        {
            description = SafeDictionary.GetValue<string>(dic, "description");
            freeTrialPeriod = SafeDictionary.GetValue<string>(dic, "freeTrialPeriod");
            iconUrl = SafeDictionary.GetValue<string>(dic, "iconUrl");
            introductoryPrice = SafeDictionary.GetValue<string>(dic, "introductoryPrice");
            introductoryPriceAmountMicros = SafeDictionary.GetValue<long>(dic, "introductoryPriceAmountMicros");
            introductoryPriceCycles = SafeDictionary.GetValue<int>(dic, "introductoryPriceCycles");
            originJson = SafeDictionary.GetValue<string>(dic, "originJson");
            originPrice = SafeDictionary.GetValue<string>(dic, "originPrice");
            originPriceAmountMicros = SafeDictionary.GetValue<long>(dic, "originPriceAmountMicros");
            price = SafeDictionary.GetValue<string>(dic, "price");
            priceAmountMicros = SafeDictionary.GetValue<long>(dic, "priceAmountMicros");
            priceCurrencyCode = SafeDictionary.GetValue<string>(dic, "priceCurrencyCode");
            productId = SafeDictionary.GetValue<string>(dic, "productId");
            subscriptionPeriod = SafeDictionary.GetValue<string>(dic, "subscriptionPeriod");
            title = SafeDictionary.GetValue<string>(dic, "title");
            type = SafeDictionary.GetValue<string>(dic, "type");
        }
    }

    public class XDGRestoredPurchase
    {
        public string orderId;

        public string packageName;

        public string productId;

        public long purchaseTime;

        public string purchaseToken;

        public int purchaseState;

        public string developerPayload;

        public bool acknowledged;

        public bool autoRenewing;

        public XDGRestoredPurchase(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public XDGRestoredPurchase(Dictionary<string, object> dic)
        {
            orderId = SafeDictionary.GetValue<string>(dic, "orderId");
            packageName = SafeDictionary.GetValue<string>(dic, "packageName");
            productId = SafeDictionary.GetValue<string>(dic, "productId");
            purchaseTime = SafeDictionary.GetValue<long>(dic, "purchaseTime");
            purchaseToken = SafeDictionary.GetValue<string>(dic, "purchaseToken");
            purchaseState = SafeDictionary.GetValue<int>(dic, "purchaseState");
            developerPayload = SafeDictionary.GetValue<string>(dic, "developerPayload");
            acknowledged = SafeDictionary.GetValue<bool>(dic, "acknowledged");
            autoRenewing = SafeDictionary.GetValue<bool>(dic, "autoRenewing");
        }
    }

    public class XDGOrderInfoWrapper
    {
        public XDGError xdgError;
        public XDGOrderInfo orderInfo;

        public XDGOrderInfoWrapper(string jsonStr)
        {
            var dic = Json.Deserialize(jsonStr) as Dictionary<string, object>;
            var errorDic = SafeDictionary.GetValue<Dictionary<string, object>>(dic, "error");
            if (errorDic != null)
            {
                xdgError = new XDGError(errorDic);
            }

            orderInfo = new XDGOrderInfo(dic);
        }
    }

    public class XDGOrderInfo
    {
        public string orderId;

        public string productId;

        public string roleId;

        public string serverId;

        public string ext;

        public XDGOrderInfo(Dictionary<string, object> dic)
        {
            orderId = SafeDictionary.GetValue<string>(dic, "orderId");
            productId = SafeDictionary.GetValue<string>(dic, "productId");
            roleId = SafeDictionary.GetValue<string>(dic, "roleId");
            serverId = SafeDictionary.GetValue<string>(dic, "serverId");
            ext = SafeDictionary.GetValue<string>(dic, "ext");
        }
    }
}