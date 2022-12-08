if (Test-Path Release\lib\) {Remove-Item Release\lib\ -Recurse -Force}
New-Item "Release\lib\" -itemType Directory
Copy-Item -Force -Recurse -Path MarkdigWrapper\bin\Release\*.dll -Destination Release\lib\

function makeReleaseZip($filename, $targetPlattform)
{
	$zipName = "Release\MarkdownPanel-" + (Get-Item $filename).VersionInfo.FileVersion + "-" + $targetPlattform + ".zip"
	Compress-Archive -LiteralPath $filename, 'Release\lib\', 'License.txt', "NppMarkdownPanel\style.css" -DestinationPath $zipName -Force
}

makeReleaseZip "NppMarkdownPanel\bin\Release\MarkdownPanel.dll" "x86"
makeReleaseZip "NppMarkdownPanel\bin\Release-x64\MarkdownPanel.dll" "x64"
# pause
