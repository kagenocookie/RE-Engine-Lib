#!/bin/bash

C:/Users/An/source/repos/ChenStackTrainer/Packer/pack.sh --ms-build
cp -r bin/Release/net472/Data bin/Release/net472/zh-CN publish
cd publish
version=0.3.3
rm *.zip
mv RszTool.AppTrainer-$version-chenstack-net472.exe RszTool.App-$version-net472.exe
mv RszTool.AppTrainer-$version-chenstack-net8.exe RszTool.App-$version-net8.exe
7z.exe a RszTool.App-$version-net472.zip Data zh-CN RszTool.App-$version-net472.exe
7z.exe a RszTool.App-$version-net8.zip Data RszTool.App-$version-net8.exe
cd ..
