using System.Collections.Generic;
using TapTap.Common;
using UnityEngine;

namespace XD.Intl.Common
{
    public class LocalConfigInfo
    {
        public TapSdkConfig tapSdkConfig;

        public LocalConfigInfo(Dictionary<string, object> dic)
        {
            var configInfoDic = SafeDictionary.GetValue<Dictionary<string, object>>(dic, "tapSdkConfig");
            tapSdkConfig = new TapSdkConfig(configInfoDic);
        }
    }

    public class TapSdkConfig
    {
        public string clientId;
        public string clientSecret;
        public bool enableTapDB;
        public bool enableMoment;
        public string tapDBChannel;
        public string serverUrl;

        public TapSdkConfig(Dictionary<string, object> dic)
        {
            clientId = SafeDictionary.GetValue<string>(dic, "clientId");
            clientSecret = SafeDictionary.GetValue<string>(dic, "clientSecret");
            enableTapDB = SafeDictionary.GetValue<bool>(dic, "enableTapDB");
            enableMoment = SafeDictionary.GetValue<bool>(dic, "enableMoment");
            tapDBChannel = SafeDictionary.GetValue<string>(dic, "tapDBChannel");
            serverUrl = SafeDictionary.GetValue<string>(dic, "serverUrl");
        }
    }
}