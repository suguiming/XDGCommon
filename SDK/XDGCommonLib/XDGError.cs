using System;
using System.Collections.Generic;
using TapTap.Common;
using UnityEngine;

namespace XD.Intl.Common
{
    [Serializable]
    public class XDGError
    {
        public int code;
        public string error_msg;
        
        public XDGError(Dictionary<string,object> dic)
        {
            if (dic != null)
            {
                code = SafeDictionary.GetValue<int>(dic,"code");
                error_msg = SafeDictionary.GetValue<string>(dic,"error_msg");      
            }
        }
        
        public XDGError(int code,string error_msg)
        {
            this.code = code;
            this.error_msg = error_msg;
        }

        public string ToJSON(){
            return JsonUtility.ToJson(this);
        }
    }
}