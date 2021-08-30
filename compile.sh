#!/bin/sh

core=("Account" "Common" "Payment")
implModule=("XDGAccountLib" "XDGCommonLib" "XDGPaymentLib")

ROOT_PATH=$(cd "$(dirname "$0")" || exit; pwd)

compileDllFile() {
    cd SDK/"$1" || echo "cd SDK/${"$1"} failed" exit
    dotnet restore "$1".sln
    dotnet build -c Release
    copyReplace "$ROOT_PATH"/SDK/"$1"/bin/Release/net5.0/XD.Intl."$2".dll "$ROOT_PATH"/Assets/XD-Intl/"$2"/Plugins/XD.Intl."$2".dll
    copyReplace "$ROOT_PATH"/SDK/"$1"/bin/Release/net5.0/XD.Intl."$2".pdb "$ROOT_PATH"/Assets/XD-Intl/"$2"/Plugins/XD.Intl."$2".pdb
    
    echo "$(cd "$(dirname "$0")" || exit; pwd)"
    cd ../..
    echo "$(cd "$(dirname "$0")" || exit; pwd)"
}

copyReplace() {
    cp -r $1 $2
}

for ((i=0;i<${#implModule[@]};i++));do
    compileDllFile "${implModule[$i]}" "${core[$i]}"
done