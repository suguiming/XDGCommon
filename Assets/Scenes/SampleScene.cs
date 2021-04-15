using UnityEngine;
using TDSGlobal;

public class SampleScene : MonoBehaviour, TDSGlobalShareCallback
{
    private string productId = "输入productId";

    private string productIds = "输入查询productId";

    private string logText = "";

    private string imagePath = "输入图片地址";

    private int shareFlavors = TDSGlobalShareFlavors.FACEBOOK;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CapturePic(string path)
    {
        ScreenCapture.CaptureScreenshot(path, 0);
    }
    

    public void ShareSuccess()
    {
        Debug.Log("分享成功");
        logText = "分享成功";
    }
    public void ShareCancel()
    {
        Debug.Log("分享取消");
        logText = "分享取消";
    }
    public void ShareError(string error)
    {
        Debug.Log("分享失败:" + error);
        logText = "分享失败" + error;
    }

    private void OnGUI()
    {
        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 30
        };

        GUIStyle myLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20
        };


        GUI.depth = 0;

        productId = GUI.TextArea(new Rect(1050, 50, 200, 100), productId, myButtonStyle);

        productIds = GUI.TextArea(new Rect(1050, 220, 200, 100), productIds, myButtonStyle);

        imagePath = GUI.TextArea(new Rect(1050, 380, 200, 100), imagePath, myButtonStyle);

        GUI.Label(new Rect(850, 600, 500, 300), logText, myLabelStyle);

        if (GUI.Button(new Rect(50, 50, 200, 60), "简体", myButtonStyle))
        {
            TDSGlobalSDK.SetLanguage(TDSGlobalLanguage.ZH_CN);
            logText = "简体";
        }

        if (GUI.Button(new Rect(50, 120, 200, 60), "繁体", myButtonStyle))
        {
            TDSGlobalSDK.SetLanguage(TDSGlobalLanguage.ZH_TW);
            logText = "繁体";
        }

        if (GUI.Button(new Rect(50, 190, 200, 60), "泰文", myButtonStyle))
        {
            TDSGlobalSDK.SetLanguage(TDSGlobalLanguage.TH);
            logText = "泰文";
        }

        if (GUI.Button(new Rect(50, 270, 200, 60), "印尼", myButtonStyle))
        {
            TDSGlobalSDK.SetLanguage(TDSGlobalLanguage.ID);
            logText = "印尼";
        }

        if (GUI.Button(new Rect(50, 350, 200, 60), "英文", myButtonStyle))
        {
            TDSGlobalSDK.SetLanguage(TDSGlobalLanguage.EN);
            logText = "英文";
        }

        if (GUI.Button(new Rect(50, 430, 200, 60), "初始化", myButtonStyle))
        {
            TDSGlobalSDK.Init((initSuccess) =>
            {
                if (initSuccess)
                {
                    Debug.Log("初始化成功");
                    logText = "初始化成功";
                }
                else
                {
                    Debug.Log("初始化失败");
                    logText = "初始化失败";
                }
            });
        }

        if (GUI.Button(new Rect(50, 510, 200, 60), "登陆", myButtonStyle))
        {
            TDSGlobalSDK.Login((tdsUser) =>
            {
                Debug.Log("user:" + tdsUser.ToJSON());
                logText = "user:" + tdsUser.ToJSON();
            }, (tdsError) =>
            {
                logText = "error:" + tdsError.ToJSON();
                Debug.Log("error:" + tdsError.ToJSON());
            });
        }

        if (GUI.Button(new Rect(50, 590, 200, 60), "账户中心", myButtonStyle))
        {
            TDSGlobalSDK.UserCenter();
        }

        if (GUI.Button(new Rect(300, 50, 200, 60), "退出登陆", myButtonStyle))
        {
            TDSGlobalSDK.Logout();
        }

        if (GUI.Button(new Rect(300, 130, 200, 60), "客服", myButtonStyle))
        {
            TDSGlobalSDK.Report("serverId", "4332464624", "roleName");
        }

        if (GUI.Button(new Rect(300, 210, 200, 60), "网页支付", myButtonStyle))
        {
            TDSGlobalSDK.PayWithWeb("1", "4332464624", (error) =>
            {
                Debug.Log("pay With Web:" + error.ToJSON());
                logText = "网页支付:" + error.ToJSON();
            });
        }


        if (GUI.Button(new Rect(300, 290, 200, 60), "查询商品", myButtonStyle))
        {
            TDSGlobalSDK.QueryWithProductIds(productIds.Split(','), (sku) =>
            {
                logText = "查询商品:";
                foreach (TDSGlobalSkuDetail detail in sku)
                {
                    logText = logText + detail.ToJSON();
                }
            }, (error) =>
             {
                 logText = "查询商品错误:" + error.ToJSON();
                 Debug.Log("查询商品错误:" + error.ToJSON());
             });

        }

        if (GUI.Button(new Rect(300, 370, 200, 60), "未完成订单", myButtonStyle))
        {
            TDSGlobalSDK.QueryRestoredPurchases((list) =>
            {
                logText = "未完成订单:";
                foreach (TDSGlobalRestoredPurchases purchaes in list)
                {
                    logText = logText + JsonUtility.ToJson(purchaes);
                }
            });
        }

        if (GUI.Button(new Rect(300, 450, 200, 60), "分享UriMessage", myButtonStyle))
        {
            TDSGlobalSDK.Share(shareFlavors, "https://www.baidu.com", "message", this);
        }

        if (GUI.Button(new Rect(300, 530, 200, 60), "分享图片", myButtonStyle))
        {
            TDSGlobalSDK.Share(shareFlavors, imagePath, this);
        }

        if (GUI.Button(new Rect(300, 610, 200, 60), "添加用户状态", myButtonStyle))
        {
            TDSGlobalSDK.AddUserStatusChangeCallback((code,message) =>
            {
                Debug.Log("code:" + code);
                logText = "用户状态回调 code:" + code + "\n" + "message:" + message;
            });
        }

        if (GUI.Button(new Rect(550, 50, 200, 60), "获取用户信息", myButtonStyle))
        {
            TDSGlobalSDK.GetUser((tdsUser) =>
            {
                Debug.Log("user:" + tdsUser.ToJSON());
                logText = "user:" + tdsUser.ToJSON();
            }, (tdsError) =>
            {
                Debug.Log("error:" + tdsError.ToJSON());
                logText = "error:" + tdsError.ToJSON();
            });
        }

        if (GUI.Button(new Rect(550, 130, 200, 60), "获取SDK版本", myButtonStyle))
        {
            TDSGlobalSDK.GetVersionName((version) =>
            {
                Debug.Log("version:" + version);
                logText = "version:" + version;
            });
        }

        if (GUI.Button(new Rect(550, 210, 200, 60), "支付商品", myButtonStyle))
        {
            TDSGlobalSDK.PayWithProduct("orderId", productId, "4332464624", "serverId", "ext", (orderInfo) =>
            {
                Debug.Log("orderInfo:" + orderInfo.ToJSON());
                logText = "orderInfo:" + orderInfo.ToJSON();
            }, (tdsError) =>
            {
                Debug.Log("tdsError:" + tdsError.ToJSON());
                logText = "tdsError:" + tdsError.ToJSON();
            });
        }

        if (GUI.Button(new Rect(550, 290, 200, 60), "补单", myButtonStyle))
        {
            TDSGlobalSDK.RestorePurchase("info", "orderId", productId, "4332464624", "serverId", "ext", (info) =>
            {
                Debug.Log("补单:" + info.ToJSON());
                logText = "补单:" + info.ToJSON();
            }, (tdsError) =>
            {
                Debug.Log("tdsError:" + tdsError.ToJSON());
                logText = "tdsError:" + tdsError.ToJSON();
            });
        }

        if (GUI.Button(new Rect(550, 370, 200, 60), "获取地区", myButtonStyle))
        {
            TDSCommon.TDSCommon.GetRegionCode((isMainLand) =>
            {
                Debug.Log("是否是国内:" + isMainLand);
                logText = "是否是国内:" + isMainLand;
            });
        }

        if (GUI.Button(new Rect(550, 450, 200, 60), "跳转商店", myButtonStyle))
        {
            TDSGlobalSDK.StoreReview();
        }

        if (GUI.Button(new Rect(550, 530, 200, 60), "IOS截图", myButtonStyle))
        {
            imagePath = Application.dataPath + "/ScreenShot/ScreenShot1.png";
            CapturePic(imagePath);
        }
        
        if (GUI.Button(new Rect(800, 50, 200, 60), "FacebookShare", myButtonStyle))
        {
            shareFlavors = TDSGlobalShareFlavors.FACEBOOK;
        }
        
        if (GUI.Button(new Rect(800, 130, 200, 60), "LineShare", myButtonStyle))
        {
            shareFlavors = TDSGlobalShareFlavors.LINE;
        }
        
        if (GUI.Button(new Rect(800, 210, 200, 60), "TwitterShare", myButtonStyle))
        {
            shareFlavors = TDSGlobalShareFlavors.TWITTER;
        }
        

    }

}
