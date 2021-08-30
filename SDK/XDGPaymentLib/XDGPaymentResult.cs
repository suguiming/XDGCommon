using System.Collections.Generic;
using TapTap.Common;
using XD.Intl.Common;

namespace XD.Intl.Payment
{
    public class XDGPaymentResult
    {
        public XDGError xdgError;

        public List<SkuDetailBean> skuDetailList;

        public XDGPaymentResult(string jsonStr)
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
}