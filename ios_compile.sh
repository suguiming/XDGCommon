#!/bin/sh
 
#UNITY程序的路径#
UNITY_PATH=/Applications/Unity/Hub/Editor/$2/Unity.app/Contents/MacOS/Unity
 
#游戏程序路径#
PROJECT_PATH=$1
 
#在Unity中构建apk#
$UNITY_PATH -buildTarget iOS -batchmode -projectPath $PROJECT_PATH -executeMethod ProjectBuild.BuildForIOS -quit

echo "开始XCode build"

Project_Folder="TDSGlobalSDKUnityDemo"
#工程名字(Target名字)
Project_Name="Unity-iPhone"
#配置环境，Release或者Debug
Configuration="Release"
#加载各个版本的plist文件
EnterpriseExportOptionsPlist=$PROJECT_PATH/ExportOptions.plist

ExportIPAPath=$5

echo $4

xcodebuild -project $PROJECT_PATH/$Project_Folder/$Project_Name.xcodeproj -scheme $Project_Name -configuration $Configuration -archivePath $PROJECT_PATH/$Project_Folder/build/$Project_Name-enterprise.xcarchive clean archive build
if [ $? -ne 0 ]; then
    exit 1
fi
xcodebuild -exportArchive -archivePath $PROJECT_PATH/$Project_Folder/build/$Project_Name-enterprise.xcarchive -exportOptionsPlist $EnterpriseExportOptionsPlist -exportPath $ExportIPAPath/$4
if [ $? -ne 0 ]; then
    exit 1
fi