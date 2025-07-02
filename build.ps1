$msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
& $msbuild NppMarkdownPanel.sln /target:Clean /target:Build /p:Configuration=Release /p:Platform=x86
& $msbuild NppMarkdownPanel.sln /target:Clean /target:Build /p:Configuration=Release /p:Platform=x64