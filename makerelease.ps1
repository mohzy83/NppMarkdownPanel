$NppMdPanelDll = "NppMarkdownPanel\bin\Release-x64\NppMarkdownPanel.dll" # or ...\Release\...
($zipName = "NppMarkdownPanel-$((Get-Item $NppMdPanelDll).VersionInfo.FileVersion).zip")

$dest = "NppMarkdownPanel\bin\Release"
Remove-Item "$dest*\*.pdb"
foreach ($d in ("$dest-x64", "$dest")) { # $dest+"-x64", $dest+""
  foreach ($f in ("style.css", "RegExp3.txt", "..\README.md", "..\License.txt")) {
    Copy-Item "NppMarkdownPanel\$f" -Destination $d -Force
  }
}
$ResDir = "$dest-tests" 
if ( !(Test-Path -Path $ResDir)) {new-item -itemtype directory $ResDir}
foreach ($d in ("test01_AπCÊE","_posts","assets")) {
  Copy-Item "NppMarkdownPanel\Resources\$d" -Destination $ResDir -Recurse -Force
}

Compress-Archive -Path "$dest*" <#only paths below "...bin\"#> -DestinationPath $zipName -Force
