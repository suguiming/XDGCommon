#!/bin/sh
remoteUPM=0

hasGit=$(which git) #判断是否已安装git
if [ ! $hasGit ]; then
  echo 'Please download git first!'
  exit 1
else
  # 获取当前分支
  currentBranch=$(git symbolic-ref --short -q HEAD)
  # 截取分支名
  git branch -D upm

  git config --local http.postBuffer 524288000  

  //强制切换到当前mr分支
  git checkout $currentBranch --force
  
  # 删除gitlab的tag以及本地缓存的Tag
  git tag -d $(git tag)

  git subtree split --prefix=TDSGlobal --branch upm

  git checkout upm

  git remote add upm git@github.com:xindong/TDS_GLOBAL_UPM.git
  
  git fetch --unshallow upm

  # 重新打tag
  git tag $1

  git push upm upm --tags --force

  git checkout $currentBranch --force

fi
