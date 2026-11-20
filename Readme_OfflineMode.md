# NppMarkdownPanel WebView2 Offline-mode Changes

## Purpose

This change adds an optional WebView2 Offline-mode to NppMarkdownPanel. The mode is intended for offline, restricted, and firewalled systems where failed network requests caused significant delays while rendering Markdown previews.

The original implementation inserted the following remote Mermaid dependency into rendered preview pages:

```text
https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.min.js
```

On systems without internet access, attempts to load that file could delay preview rendering. Markdown content could also initiate other browser requests through remote images, stylesheets, scripts, frames, media, links, WebSockets, fetch requests, and similar browser functionality.

No NuGet dependencies were added or changed.

## Modified Files

The implementation changes the following files:

```text
NppMarkdownPanel\Entities\Settings.cs
NppMarkdownPanel\Forms\SettingsForm.cs
NppMarkdownPanel\Forms\MarkdownPreviewForm.cs
NppMarkdownPanel\MarkdownPanelController.cs
PanelCommon\PanelCommon.csproj
Webview2Viewer\Webview2WebbrowserControl.cs
```

The following shared helper was added:

```text
PanelCommon\WebviewResourceConstants.cs
```

## New Settings

Two settings were added:

```text
OfflineMode
OfflineMermaidScriptFileName
```

They are stored in the plugin INI file under the `Options` section:

```ini
[Options]
OfflineMode=True
OfflineMermaidScriptFileName=C:\Approved\WebAssets\mermaid.min.js
```

The default value of `OfflineMode` is `False`, preserving the original online behavior unless the user explicitly enables the option.

## Settings Interface

A new **WebView2 Offline-mode** section appears at the bottom of the MarkdownPanel settings dialog.

It contains:

1. **Block non-local WebView2 requests and background networking**

   Enables or disables Offline-mode.

2. **Local Mermaid JS file**

   Optionally specifies an approved local copy of `mermaid.min.js`.

The local Mermaid selector is enabled only when Offline-mode is selected.

The selected Mermaid file must:

- Exist.
- Have a `.js` extension.
- Be specified using an absolute path.
- Be stored on a local drive.
- Not use a UNC path.
- Not reside on a mapped network drive.

Invalid paths are rejected when the settings are saved.

## Using Offline-mode

1. Open Notepad++.
2. Select:

   ```text
   Plugins
   → MarkdownPanel
   → Settings
   ```

3. Under **WebView2 Offline-mode**, enable:

   ```text
   Block non-local WebView2 requests and background networking
   ```

4. To use Mermaid diagrams, obtain and approve a local copy of:

   ```text
   mermaid.min.js
   ```

   The original plugin used Mermaid version 11 from:

   ```text
   mermaid@11/dist/mermaid.min.js
   ```

5. Store the approved JavaScript file on a local drive, for example:

   ```text
   C:\Approved\WebAssets\mermaid.min.js
   ```

6. Select that file using the **Local Mermaid JS file** browse button.
7. Save the settings.

The WebView2 instance is recreated automatically when Offline-mode or the local Mermaid path changes.

## Using Offline-mode Without Mermaid

The local Mermaid file is optional.

When Offline-mode is enabled and no valid local Mermaid file is configured:

- No online Mermaid request is attempted.
- Mermaid support is disabled.
- Normal Markdown rendering continues.
- Mermaid code blocks will not be rendered as diagrams.

There is no network fallback.

## Online-mode Behavior

When Offline-mode is disabled, the original online behavior is retained.

The preview loads Mermaid from:

```text
https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.min.js
```

The additional Offline-mode request filtering and Content Security Policy are not applied.

## Offline Mermaid Loading

For browser previews, the selected local Mermaid file is exposed to WebView2 through a local virtual host:

```text
https://markdownpanel-offline.local/
```

The virtual host maps to the local directory containing the approved Mermaid file.

The mapping uses WebView2's `DenyCors` access mode. This allows the browser to load the file as a script while preventing CORS-enabled access such as `fetch` or `XMLHttpRequest` against the mapped directory.

For saved HTML exports, the generated HTML references the selected Mermaid file using an absolute file URI. The JavaScript is not embedded into the exported HTML.

Consequently, an exported HTML file using Mermaid will continue to depend on the configured local Mermaid file remaining at the same path.

## Content Security Policy

Offline preview pages receive a restrictive Content Security Policy.

The policy:

- Denies resources by default.
- Allows inline preview styles.
- Allows inline preview scripts required by the plugin.
- Allows data URLs for embedded content.
- Allows local file resources.
- Allows resources from the local Markdown document virtual host.
- Allows scripts from the offline Mermaid virtual host.
- Disables network connections through `connect-src`.
- Disables frames.
- Disables embedded objects.
- Disables forms.
- Disables alternate base URLs.

This provides a second layer of protection in addition to WebView2 request interception.

## WebView2 Request Filtering

When Offline-mode is enabled, the plugin installs a `WebResourceRequested` filter covering all WebView2 resource contexts.

Requests are allowed only for:

- The local Markdown document virtual host.
- The local offline-assets virtual host used by Mermaid.
- Local file URLs that do not reference network paths.
- `about:` URLs needed internally by WebView2.
- `data:` URLs.
- `blob:` URLs.

Other requests are blocked immediately with a local HTTP 403 response:

```text
Blocked by NppMarkdownPanel Offline-mode.
```

Blocked categories include:

- External HTTP and HTTPS resources.
- Remote images.
- Remote scripts.
- Remote stylesheets.
- Remote fonts.
- Remote media.
- Remote frames.
- WebSocket connections.
- FTP resources.
- Custom URL protocols.
- Other unrecognized schemes.

## Navigation and Popup Blocking

When Offline-mode is enabled:

- External page navigation is cancelled.
- New-window and popup requests are marked as handled and are not opened.
- Local document links continue to open through Notepad++ where permitted.
- Links to files on UNC paths or mapped network drives are rejected.

## Local Markdown Resources

The plugin continues to support local resources associated with the current Markdown document, such as local images.

In Offline-mode, the Markdown document directory is mapped only when the document is located on a local drive.

Documents located on UNC paths or mapped network drives are not mapped because accessing them could create network activity.

The local document mapping uses `DenyCors` in Offline-mode.

## Separate WebView2 Profile

Offline-mode uses a separate WebView2 profile directory named:

```text
webview2-offline
```

Normal mode continues to use:

```text
webview2
```

This prevents the online and offline configurations from sharing the same WebView2 profile state.

## Background Networking Controls

When Offline-mode is enabled, the WebView2 environment is started with browser arguments intended to reduce or disable Chromium background network services.

The configured arguments disable or reduce:

- Background networking.
- Component updates.
- Domain reliability reporting.
- Browser synchronization.
- Default application activity.
- Client-side phishing detection.
- DNS prefetching.
- Breakpad crash reporting.
- First-run activity.
- Optimization hints.
- Media Router activity.
- Translation services.
- Autofill server communication.
- Certificate Transparency component updates.

Where supported by the installed WebView2 SDK and runtime, the plugin also requests:

- Custom crash reporting instead of the default WebView2 reporting path.
- Disabling SmartScreen reputation checks.

These optional properties are applied through reflection so the plugin remains compatible with runtimes that do not expose them. The request interceptor and Content Security Policy remain active even if an optional runtime property is unavailable.

## Runtime Scope

Offline-mode controls requests made by the WebView2 instance hosted by NppMarkdownPanel.

It cannot control separate machine-level services outside the plugin process, such as the updater belonging to an installed Evergreen WebView2 Runtime.

Systems requiring a strict machine-wide zero-egress guarantee should continue to enforce an outbound firewall policy or use an appropriately managed WebView2 runtime deployment.

## Settings Changes

Changing either of the following settings causes the existing WebView2 control to be disposed and recreated:

```text
OfflineMode
OfflineMermaidScriptFileName
```

This is required because WebView2 environment options, profile paths, virtual-host mappings, and request filters are established during initialization.

## Expected Result

With Offline-mode enabled:

- The Mermaid CDN is not contacted.
- Remote Markdown resources are not contacted.
- External navigation and popup activity are blocked.
- Disallowed requests fail immediately rather than waiting for network timeouts.
- Preview rendering remains functional using local content.
- Mermaid rendering remains available when an approved local Mermaid JavaScript file is configured.
