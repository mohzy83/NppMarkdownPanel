function makeReleaseZip($filename, $targetPlattform)
{
	$zipName = "NppMarkdownPanel\bin\NppMarkdownPanel-" + (Get-Item $filename).VersionInfo.FileVersion + "-" + $targetPlattform + ".zip"
	Compress-Archive -LiteralPath $filename, 'License.txt', "NppMarkdownPanel\style.css" -DestinationPath $zipName -Force
}

makeReleaseZip "NppMarkdownPanel\bin\Release\MarkdownPanel.dll" "x86"
makeReleaseZip "NppMarkdownPanel\bin\Release-x64\MarkdownPanel.dll" "x64"