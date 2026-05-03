# AGENTS.md

## Build
```powershell
.\build.ps1
```
Builds Release x86 and x64 via MSBuild. `Copy-Output-Debug-*.cmd` scripts deploy the DLL to the Notepad++ plugins folder for testing.

## Architecture

4 projects in `NppMarkdownPanel.sln`:
| Project | Role |
|---------|------|
| `NppMarkdownPanel` | Notepad++ plugin DLL + Windows Forms UI |
| `MarkdigWrapper` | Markdig pipeline: markdown → HTML with syntax highlighting |
| `PanelCommon` | Shared interfaces: `IMarkdownGenerator`, `IWebbrowserControl` |
| `Webview2Viewer` | Edge/Chromium WebView2 rendering engine |

## Rendering Pipeline

1. `MarkdownPanelController` detects Scintilla changes, debounces (400ms threshold, 500ms cycle)
2. `MarkdownService.ConvertToHtml()` → optional pre-processor → `MarkdigMarkdownGenerator.ConvertToHtml()` → optional post-processor
3. `MarkdigMarkdownGenerator` builds a pipeline with: AdvancedExtensions, YamlFrontMatter, SyntaxHighlighting, PreciseSourceLocation. Sets `id` attributes on every block for scroll sync.
4. `MarkdownPreviewForm.RenderHtmlInternal()` wraps the HTML body in `DEFAULT_HTML_BASE` template
5. `IWebbrowserControl.SetContent()` renders in the web view

## Two Rendering Engines

- **WebView2 (EDGE)** — `Webview2WebbrowserControl.cs` — default, modern browser features
- **IE11 (WebView1)** — `IE11WebbrowserControl.cs` — legacy fallback, no Mermaid support

Default engine set in `Settings.RenderingEngine = RENDERING_ENGINE_WEBVIEW2_EDGE`.

## HTML Template

Hardcoded in `NppMarkdownPanel/Forms/MarkdownPreviewForm.cs:DEFAULT_HTML_BASE`. Four `string.Format` placeholders: `{0}`=title, `{1}`=CSS, `{2}`=body style, `{3}`=body HTML. Scripts for Mermaid are embedded directly in the template.

## Mermaid Diagrams

- **Renderer**: `SyntaxHighlightingCodeBlockRenderer.cs` detects `mermaid` language, outputs `<pre class="mermaid">` (HTML-escaped, no syntax highlighting). Other fenced code blocks get syntax-highlighted `<div>` wrappers.
- **JS library**: Mermaid v10 loaded via CDN (`jsdelivr`) in the HTML template `<head>`. Initialized with `startOnLoad: false`, then `mermaid.run()` is called at end of `<body>`.
- **WebView2 partial updates**: After dynamic `document.body.innerHTML` injection, `mermaid.run()` is called again via `ExecuteScriptAsync`.
- **IE11**: Mermaid v10 does not support IE11. Diagrams silently fail to render (not an error).

## Key File Locations

- HTML template: `NppMarkdownPanel/Forms/MarkdownPreviewForm.cs:19`
- Mermaid renderer logic: `MarkdigWrapper/Markdig/SyntaxHighlighting/SyntaxHighlightingCodeBlockRenderer.cs:44`
- Markdig pipeline: `MarkdigWrapper/MarkdigMarkdownGenerator.cs:30`
- WebView2 body update + Mermaid re-run: `Webview2Viewer/Webview2WebbrowserControl.cs:198`
- Scroll sync JS injection: `Webview2Viewer/Webview2WebbrowserControl.cs:126`
- Settings entity: `NppMarkdownPanel/Entities/Settings.cs`
- Notepad++ integration: `NppMarkdownPanel/PluginInfrastructure/`

## CSS

- `NppMarkdownPanel/style.css` — light theme
- `NppMarkdownPanel/style-dark.css` — dark theme
- Users can override via custom CSS file path in settings

## Test Files

- `NppMarkdownPanel/Resources/nppMdP.tests/` — sample `.md` and `.html` files
- Test Mermaid: `Test-MD.md` — general test; embed ```mermaid blocks in any test file to verify rendering
