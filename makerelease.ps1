if (Test-Path Release\lib\) {Remove-Item Release\lib\ -Recurse -Force}
New-Item "Release\lib\" -itemType Directory
Copy-Item -Force -Recurse -Path MarkdigWrapper\bin\Release\*.dll -Destination Release\lib\
Copy-Item -Force -Recurse -Path PanelCommon\bin\Release\*.dll -Destination Release\lib\
Copy-Item -Force -Recurse -Path Webview2Viewer\bin\Release\*.dll -Destination Release\lib\
Copy-Item -Force -Recurse -Path Webview2Viewer\bin\Release\runtimes\ -Destination Release\lib\runtimes\
# Offline math rendering assets, vendored by the CI workflow before this script runs.
# The WebView2 preview loads them through the markdownpanel-assets virtual host.
if (Test-Path NppMarkdownPanel\mathjax\) { Copy-Item -Force -Recurse -Path NppMarkdownPanel\mathjax\ -Destination Release\lib\mathjax\ }
if (Test-Path NppMarkdownPanel\mathjax-fonts\) { Copy-Item -Force -Recurse -Path NppMarkdownPanel\mathjax-fonts\ -Destination Release\lib\mathjax-fonts\ }

function makeReleaseZip($filename, $targetPlattform)
{
	$zipName = "Release\NppMarkdownPanel-" + (Get-Item $filename).VersionInfo.FileVersion + "-" + $targetPlattform + ".zip"
	Compress-Archive -LiteralPath $filename, 'Release\lib\', 'README.md', 'help\', 'License.txt', "NppMarkdownPanel\style.css" , "NppMarkdownPanel\style-dark.css" -DestinationPath $zipName -Force
}

makeReleaseZip "NppMarkdownPanel\bin\Release\NppMarkdownPanel.dll" "x86"
makeReleaseZip "NppMarkdownPanel\bin\Release-x64\NppMarkdownPanel.dll" "x64"
pause