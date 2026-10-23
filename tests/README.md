Renderer regression checks

Requires MSBuild and the .NET Framework 4.7.2 developer pack. From the repository root:

```powershell
msbuild NppMarkdownPanel.sln /t:Restore /p:RestorePackagesConfig=true
$codePages = (Resolve-Path "packages/System.Text.Encoding.CodePages.6.0.0/lib/net461").Path
msbuild tests/RendererRegressionTests.csproj /p:Configuration=Release "/p:ReferencePath=$codePages"
.\tests\bin\Release\RendererRegressionTests.exe
```

The runner generates a 1,500-row table with five bold cells per row and checks
row retention, formatting, source line attributes, export, wider rows, ordinary
paragraphs, HTML sanitization, and the nesting-depth limit. No input file is needed.

The CodePages search path compensates for the existing project's `net462` hint:
that package provides its .NET Framework assembly under `net461`.
