using System.Collections.Generic;
using TapTap.Common;
using UnityEngine;

namespace XD.Intl.Common
{
    /**
     * 桥接返回值只有一个布尔类型值返回的转换类
     */
    public class XDGBooleanContentWrapper
    {
        public bool isSuccess;

        public XDGBooleanContentWrapper(string resultJson)
        {
            if (Json.Deserialize(resultJson) is Dictionary<string, object> dic) isSuccess = (bool)dic["success"];
        }
    }

    public class XDGInitResultWrapper
    {
        public bool isSuccess;

        public LocalConfigInfo localConfigInfo;

        public XDGInitResultWrapper(string resultJson)
        {
            var dic = Json.Deserialize(resultJson) as Dictionary<string, object>;
            isSuccess = SafeDictionary.GetValue<bool>(dic, "success");
            var configInfoDic = SafeDictionary.GetValue<Dictionary<string, object>>(dic, "configInfo");
            localConfigInfo = new LocalConfigInfo(configInfoDic);
        }
    }

    public class XDGShareResultWrapper
    {
        public bool cancel;

        public XDGError error;

        public XDGShareResultWrapper(string json)
        {
            Dictionary<string, object> dic = Json.Deserialize(json) as Dictionary<string, object>;
            cancel = SafeDictionary.GetValue<bool>(dic, "cancel");
            Dictionary<string, object> errorDic = SafeDictionary.GetValue<Dictionary<string, object>>(dic, "error");
            if (errorDic != null)
            {
                error = new XDGError(errorDic);
            }
        }

        public string ToJSON()
        {
            return JsonUtility.ToJson(this);
        }
    }
}