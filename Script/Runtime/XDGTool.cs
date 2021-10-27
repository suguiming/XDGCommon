using System;
using TapTap.Common;
using UnityEngine;

namespace XD.Intl.Common{
    public class XDGTool{
        public static void Log(string msg){
            Debug.Log("\n------------------ XDGSDK打印信息 ------------------\n"+msg + "\n\n");
        }
        
        public static void LogError(string msg){
            Debug.LogError("\n------------------ XDGSDK报错 ------------------\n"+msg + "\n\n");
        }
        
        public  static  bool checkResultSuccess(Result result){
            return result.code == Result.RESULT_SUCCESS && !string.IsNullOrEmpty(result.content);
        }
    }
}