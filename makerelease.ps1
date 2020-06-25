# function makeReleaseZip($filename, $targetPlattform)
# {
# 	$zipName = "NppMarkdownPanel-" + (Get-Item $filename).VersionInfo.FileVersion + "-" + $targetPlattform + ".zip"
# 	Compress-Archive -LiteralPath $filename, 'README.md', 'License.txt', "NppMarkdownPanel\style.css" , "NppMarkdownPanel\RegExp3.txt" -DestinationPath $zipName -Force
# }
# makeReleaseZip "NppMarkdownPanel\bin\Release\NppMarkdownPanel.dll" "x86"
# makeReleaseZip "NppMarkdownPanel\bin\Release-x64\NppMarkdownPanel.dll" "x64"

$NppMdPanelDll = "NppMarkdownPanel\bin\Release-x64\NppMarkdownPanel.dll"
$dest = "NppMarkdownPanel\bin\Release"

Remove-Item "$dest*\*.pdb"
foreach ($d in ("$dest-x64", "$dest")) {
  foreach ($f in ("style.css", "RegExp3.txt", "..\README.md", "..\License.txt")) {
    Copy-Item "NppMarkdownPanel\$f" -Destination $d -Force
  }
}
($zipName = "NppMarkdownPanel-$((Get-Item $NppMdPanelDll).VersionInfo.FileVersion).zip")
Compress-Archive -Path "$dest*" <#only paths below "...bin\"#> -DestinationPath $zipName -Force
