#!/bin/sh
export LANG="en_US.UTF-8"

BUILD_TYPE=$1

isNightly() {
    if [ "Nightly" = "$BUILD_TYPE" ];then
        echo "IS Nightly"
        return 0
    else
        echo "NOT Nightly"
        return 1
    fi
}

isRelease() {
    if [ "Release" = "$BUILD_TYPE" ];then
        echo "IS Release"
        return 0
    else
        echo "NOT Release"
        return 1
    fi
}

buildFail() {
    java -jar ./.ci/release.jar message --title="${CI_PROJECT_TITLE} $BUILD_TYPE build " --body="<${CI_JOB_URL}|Package Failed>"
    exit 1
}

judgeResult() {
    if [ $? -ne 0 ]; then
        if [ -z "$1" ]; then
            echo "$1"
        else
            echo ">>     failed      <<\n"
        fi
        buildFail
    fi
}

if isRelease; then
    tag=${CI_COMMIT_TAG}
    sdkVersion=${tag:1}
elif isNightly; then
    tag=${CI_COMMIT_TAG}
    echo "tag is $tag"
    sdkVersion=${tag:3}
fi

java -jar ./.ci/release.jar message --title="${CI_PROJECT_TITLE} $BUILD_TYPE build " --body="<${CI_JOB_URL}|Package Start>"

project_dir=$(pwd)

FILE=$project_dir/Packages/manifest.json

ios_compile_sh=$project_dir/ios_compile.sh

android_compile_sh=$project_dir/android_compile.sh

unity_version=2020.1.17f1c1

isRND=false

doBuild() {

    cd $1

    echo "$2 | --------------start $2 build--------------"

    upmVersion=`grep "TAPSDK_UPM.git#" $FILE | sed "s/.*TAPSDK_UPM.git#\(.*\)\",/\1/"`

    if [ "$3" = "Android" ]; then
        sh $android_compile_sh $project_dir $unity_version $isRND $upmVersion $(pwd) 
    else 
        sh $ios_compile_sh $project_dir $unity_version $isRND $upmVersion $(pwd)
    fi

    if [ $? -ne 0 ]; then
        echo "$2 | --------------build $2 failed--------------"
    else
        echo "$2 | --------------build $2 success--------------"
    fi

    cd ..
    pwd
    echo "$2 | list archives"
    ls -l $1

    echo pwd

}


PRODUCT_DIR=./Products
RELEASE_DIR=./release

# 生成Product目录
mkdir -p $PRODUCT_DIR
cd $PRODUCT_DIR

mkdir -p $RELEASE_DIR

doBuild $RELEASE_DIR "Release" "Android"
# doBuild $RELEASE_DIR "Release" "IOS"

cd ..
ls -l $PRODUCT_DIR
zip -q -ry ${CI_PROJECT_TITLE}-$sdkVersion.zip $PRODUCT_DIR
judgeResult "ZIP FAIL"

echo "send to slack end"
if isNightly; then
    java -jar .ci/release.jar nb --af=${CI_PROJECT_TITLE}-$sdkVersion.zip --ver=$sdkVersion
else
    java -jar .ci/release.jar release --af=${CI_PROJECT_TITLE}-$sdkVersion.zip --ver=$sdkVersion
fi
judgeResult