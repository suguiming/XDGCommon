using System;
using System.Collections.Generic;
using TapTap.Common;
using UnityEngine;
using XD.Intl.Common;

namespace XD.Intl.Payment
{
    [Serializable]
    public class XDGProductWrapper
    {
        public List<XDGProduct> productList;
        public XDGError error;

        public XDGProductWrapper(string json)
        {
            Dictionary<string,object> dic = Json.Deserialize(json) as Dictionary<string,object>;
            Dictionary<string,object> errorDic = SafeDictionary.GetValue<Dictionary<string,object>>(dic,"error");
            List<object> list = SafeDictionary.GetValue<List<object>>(dic,"products");

            if (errorDic != null)
            {
                this.error = new XDGError(errorDic);
            }

            if (list != null)
            {
                this.productList = new List<XDGProduct>();
                foreach(var detailDic in list)
                {
                    Dictionary<string,object> innerDic = detailDic as Dictionary<string,object>;
                    productList.Add( new XDGProduct(innerDic));
                }
            }
        }
    }

    [Serializable]
    public class XDGProduct
    {
        public string localizedDescription;

        public string localizedTitle;

        public double price;

        public string productIdentifier;

        public PriceLocale priceLocale;
        
        public XDGProduct(Dictionary<string,object> dic)
        {
            this.localizedDescription = SafeDictionary.GetValue<string>(dic,"localizedDescription");
            this.localizedTitle = SafeDictionary.GetValue<string>(dic,"localizedTitle") ;
            this.price =SafeDictionary.GetValue<double>(dic,"price");
            this.productIdentifier = SafeDictionary.GetValue<string>(dic,"productIdentifier");
           
            Dictionary<string,object> priceLocaleDic = SafeDictionary.GetValue<Dictionary<string,object>>(dic,"priceLocale");
            this.priceLocale = new PriceLocale(priceLocaleDic);
        }
        
    }

    [Serializable]
    public class PriceLocale
    {
        public string localeIdentifier;

        public string languageCode;

        public string countryCode;

        public string scriptCode;

        public string calendarIdentifier;

        public string decimalSeparator;

        public string currencySymbol;
        
        public PriceLocale(Dictionary<string,object> dic)
        {
            this.localeIdentifier = SafeDictionary.GetValue<string>(dic,"localeIdentifier");
            this.languageCode = SafeDictionary.GetValue<string>(dic,"languageCode");
            this.countryCode = SafeDictionary.GetValue<string>(dic,"countryCode");
            this.scriptCode = SafeDictionary.GetValue<string>(dic,"scriptCode");
            this.calendarIdentifier = SafeDictionary.GetValue<string>(dic,"calendarIdentifier");
            this.decimalSeparator = SafeDictionary.GetValue<string>(dic,"decimalSeparator");
            this.currencySymbol = SafeDictionary.GetValue<string>(dic,"currencySymbol");
        }
    }
}