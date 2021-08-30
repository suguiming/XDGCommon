using UnityEngine;
using XD.Intl.Account;
using XD.Intl.Common;

public class SampleScene : MonoBehaviour, XDGShareCallback{
    private string productId = "输入productId";
    private string productIds = "输入查询productId";
    private string imagePath = "输入图片地址";
    private string loginType = "0";
    private ShareFlavors shareFlavors = 0;

    private GUIStyle _myButtonStyle;
    private string logText = "";

    private int screenWidth; // 1080
    private int screenHeight; // 1920

    private void Awake(){
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        XDGTool.Log("设备宽高: screenWidth:" + screenWidth + ", screenHeight:" + screenHeight);
    }

    private void Start(){
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    private void OnGUI(){
        RefreshButton();
    }

    private void RefreshButton(){
        GUI.depth = 0;
        if (_myButtonStyle == null){
            _myButtonStyle = new GUIStyle(GUI.skin.button){
                fontSize = 30
            };
        }

        var myLabelStyle = new GUIStyle(GUI.skin.label){
            fontSize = 40,
            normal ={
                textColor = Color.white
            }
        };
        GUI.Label(new Rect(50, (int) screenWidth - 100, screenWidth, 100), logText, myLabelStyle);

        if (GUI.Button(NewButtonRectAuto(0), "简体", _myButtonStyle)){
            XDGCommon.SetLanguage(LangType.ZH_CN);
            logText = "简体";
        }

        if (GUI.Button(NewButtonRectAuto(1), "繁体", _myButtonStyle)){
            XDGCommon.SetLanguage(LangType.ZH_TW);
            logText = "繁体";
        }

        if (GUI.Button(NewButtonRectAuto(2), "泰文", _myButtonStyle)){
            XDGCommon.SetLanguage(LangType.TH);
            logText = "泰文";
        }

        if (GUI.Button(NewButtonRectAuto(3), "印尼", _myButtonStyle)){
            XDGCommon.SetLanguage(LangType.ID);
            logText = "印尼";
        }

        if (GUI.Button(NewButtonRectAuto(4), "英文", _myButtonStyle)){
            XDGCommon.SetLanguage(LangType.EN);
            logText = "英文";
        }

        if (GUI.Button(NewButtonRectAuto(5), "初始化", _myButtonStyle)){
            XDGCommon.Init((success => {
                if (success){
                    XDGTool.Log("初始化成功");
                    logText = "初始化成功";
                }
                else{
                    XDGTool.Log("初始化失败");
                    logText = "初始化失败";
                }
            }));
        }

        if (GUI.Button(NewButtonRectAuto(6), "登录", _myButtonStyle)){
            XDGAccount.Login(user => { logText = user.name; }, error => { logText = error.error_msg; });
        }

        if (GUI.Button(NewButtonRectAuto(7), "登出", _myButtonStyle)){
            XDGAccount.Logout();
        }

        if (GUI.Button(NewButtonRectAuto(8), "是否初始化", _myButtonStyle)){
            XDGCommon.IsInitialized(b => { logText = b ? "已经初始化" : "未初始化"; });
        }

        if (GUI.Button(NewButtonRectAuto(9), "用户状态", _myButtonStyle)){
            XDGAccount.AddUserStatusChangeCallback((code, msg) => { logText = "code: " + code + " msg: " + msg; });
        }

        if (GUI.Button(NewButtonRectAuto(10), "获取用户", _myButtonStyle)){
            XDGAccount.GetUser((user) => { logText = "成功: " + user.name; },
                (error) => { logText = "失败: " + error.error_msg; });
        }

        if (GUI.Button(NewButtonRectAuto(11), "打开用户中心", _myButtonStyle)){
            XDGAccount.OpenUserCenter();
        }

        if (GUI.Button(NewButtonRectAuto(12), "客服", _myButtonStyle)){
            XDGCommon.Report("serverId", "4332464624", "roleName");
        }

        if (GUI.Button(NewButtonRectAuto(13), "分享UriMessage", _myButtonStyle)){
            XDGCommon.Share(shareFlavors, "https://www.baidu.com", "message", this);
        }

        if (GUI.Button(NewButtonRectAuto(14), "分享图片", _myButtonStyle)){
            XDGCommon.Share(shareFlavors, imagePath, this);
        }

        if (GUI.Button(NewButtonRectAuto(15), "获取SDK版本", _myButtonStyle)){
            XDGCommon.GetVersionName((version) => {
                XDGTool.Log("version:" + version);
                logText = "version:" + version;
            });
        }

        if (GUI.Button(NewButtonRectAuto(16), "跳转商店", _myButtonStyle)){
            XDGCommon.StoreReview();
        }

        if (GUI.Button(NewButtonRectAuto(17), "IOS截图", _myButtonStyle)){
            imagePath = Application.dataPath + "/ScreenShot/ScreenShot1.png";
            CapturePic(imagePath);
        }

        if (GUI.Button(NewButtonRectAuto(18), "FacebookShare", _myButtonStyle)){
            shareFlavors = ShareFlavors.FACEBOOK;
        }

        if (GUI.Button(NewButtonRectAuto(19), "LineShare", _myButtonStyle)){
            shareFlavors = ShareFlavors.LINE;
        }

        if (GUI.Button(NewButtonRectAuto(20), "TwitterShare", _myButtonStyle)){
            shareFlavors = ShareFlavors.TWITTER;
        }
    }

    private void CapturePic(string path){
        ScreenCapture.CaptureScreenshot(path, 0);
    }

    public void ShareSuccess(){
        XDGTool.Log("分享成功");
        logText = "分享成功";
    }

    public void ShareCancel(){
        XDGTool.Log("分享取消");
        logText = "分享取消";
    }

    public void ShareFailed(string error){
        XDGTool.Log("分享失败:" + error);
        logText = "分享失败" + error;
    }

    private const int ButtonWid = 250;
    private const int ButtonHei = 90;

    private Rect NewButtonRectAuto(int index){
        var rect = new Rect(30 + (ButtonWid + 30) * (index / 8), 30 + (ButtonHei + 30) * (index % 8), ButtonWid,
            ButtonHei);
        return rect;
    }
}