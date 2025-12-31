$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuildpath = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
$msbuild = join-path $msbuildpath 'MSBuild\Current\Bin\MSBuild.exe'
& $msbuild NppMarkdownPanel.sln /restore /p:RestorePackagesConfig=true /target:Clean /target:Build /p:Configuration=Release /p:Platform=x86
& $msbuild NppMarkdownPanel.sln /restore /p:RestorePackagesConfig=true /target:Clean /target:Build /p:Configuration=Release /p:Platform=x64