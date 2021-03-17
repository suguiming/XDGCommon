#!/bin/sh
 
echo Unity Version:$2
#UNITY程序的路径#
UNITY_PATH=/Applications/Unity/Hub/Editor/$2/Unity.app/Contents/MacOS/Unity
#在Unity中构建apk#
$UNITY_PATH -buildTarget Android -batchmode -projectPath $1 -executeMethod ProjectBuild.BuildForAndroid -UNITY_VERSION=$2 -IS_RND=$3 -UPM_VERSION=$4 -EXPORT_PATH=$5 -quit

if [ $? -ne 0 ]; then
    exit 1
else
    echo "Apk生成完毕" 
fi
