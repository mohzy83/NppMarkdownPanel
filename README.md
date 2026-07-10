# MarkdownPanel for Notepad++
Plugin to preview Markdown files in Notepad++

- lightweight plugin to preview markdown within Notepad++
- displaying rendered markdown HTML with **WebView2 Edge** (since 0.9.0) or an embedded IE11
- can save rendered html to a file
- Dark mode support (_requires Notepad++ version 8.4.1 or later_)

### Current Version

The current version is **0.9.2** it can be found [here](https://github.com/mohzy83/NppMarkdownPanel/releases)


## Prerequisites
- .NET 4.7.2 or higher

## Installation
### Installation in Notepad++ 
The plugin can be installed with the Notepad++ Plugin Admin.
The name of the plugin is **Markdown Panel**.

### Manual Installation
Create the folder "NppMarkdownPanel" in your Notepad++ plugin folder (e.g. "C:\Program Files\Notepad++\plugins") and extract the appropriate zip (x86 or x64) to it.

It should look like this:

![pluginfolder](help/pluginfolder.png "Layout of the plugin folder after installation")

**Issues with manual installation:**
Windows blocks downloaded DLLs by default. That means you likely get the following error message: 

> Failed to load <br>
> NppMarkdownPanel.dll is not compatible with the current version of Notepad++
	
Make sure to unblock __all__ DLLs of the plugin (also DLLs in subfolders).
![npp-unblock](help/npp-unblock.png "Unblock all DLLs")

For an detailed explanation check issue [57](https://github.com/mohzy83/NppMarkdownPanel/issues/57).

**Note for Windows 7 users:**
 WebView2 Edge is required for the plugin to function properly. 
 Windows 7 does not include WebView2 Edge by default, so you must manually install the WebView2 Runtime from Microsoft's WebView2 download page before using the plugin.
 https://developer.microsoft.com/en-us/microsoft-edge/webview2?form=MA13LH#download

## Usage

After the installation you will find a small purple markdown icon in your toolbar.
Just click it to show the markdown preview. Click again to hide the preview.
Thats all you need to do ;)

![npp-preview](help/npp-preview.png "Markdown preview with standard CSS")

With dark mode enabled in Notepad++:

![npp-preview-dm](help/npp-preview-dm.png "Markdown preview with darkmode CSS")


### Settings

To open the settings for this plugin: Plugins -> MarkdownPanel -> Settings
![npp-settings](help/open-settings.png "open settings dialog")

* #### HTML Rendering Engine
	This option allows you to switch between two HTML Rendering Engines.
	
	1. Edge (WebView2)	
	2. Internet Explorer 11 (WebView1)
	
	Default option is **Edge (WebView2)**. It is a modern browser implementation based on Chromium 
	and it supports modern web standards. IE11 option is still available, but not recommended anymore 
	due to its age.
	
	![settings-rendering](help/settings-rendering.png "settings Rendering")

* #### CSS File
    This allows you to select a CSS file to use if you don't want the default style of the preview
	
* #### Dark mode CSS File
	This allows you to select a Dark mode CSS file. When the Notepad++ dark mode is enabled, this Css file is used.
	When no file is set, the default dark mode Css is used.

* #### Zoom Level
    This allows you to set the zoom level of the preview

* #### Automatic HTML Output
    This allows you to select a file to save the rendered HTML to every time the preview is rendered. This is a way to automatically save the rendered content to use elsewhere. Leaving this empty disables the automatic saving.  
    __Note: This is a global setting, so all previewed documents will save to the same file.__

* #### Allow all file extensions
   This option allows you to skip file extension checking. Every active file will be processed by the markdown converter.
   But be careful, this option may have undesired effects. (e.g. rendering large logs or large source code files can be slow)
   The input field for supported file extensions is disabled when this option is checked.

* #### Supported File Extensions
    This allows you to define a list of file extensions, which are supported and displayed in Markdown Panel.
	Other file type won't be displayed (there will be a warning).
	The file extensions have to be separated by a comma `,` - character.
	No input allowed when option "Allow all file extensions" is checked.
	
* #### Enable preview for files without extension
    When this option is checked, Markdown Panel renders files without an extension (eg. "new 2").
	
* #### Automatically show panel for supported files
    When this option is checked, Markdown Panel will open the preview window automatically for files with a supported extension.
	The preview will be closed for files with no supported extension.
	

* #### Show Toolbar in Preview Window
    Checking this box will enable the toolbar in the preview window. By default, this is unchecked.

* #### Show Statusbar in Preview Window (Preview Links)
    Checking this box will show the status bar, which previews urls for links. By default, this is unchecked.

* #### Enable three-state toggle (docked → fullscreen → hidden)
	This option changes the behaviour of the "Toggle Markdown Panel" function. When this option is checked, the panel can be
	switched between the states hidden, docked and fullscreen. When this option in not checked, the panel toggles between the states docked and hidden.

### Preview Window Toolbar

* #### Save As... (![save-btn](help/save-btn.png "Picture of the Save button on the preview panel toolbar"))
    Clicking this button allows you to save the rendered preview as an HTML document.

* #### Save As (with Light Theme)
    Saves the rendered preview as an HTML document using the light theme (even if the darkmode is enabled).

![/saveas_lighttheme](help/saveas_lighttheme.png "Save As Light Theme")	

* #### Copy to clipboard
    Copies the current preview to the clipboard (formatted in HTML). 
	
* #### Export to PDF
    Exports the current preview as PDF file. 

### Synchronize viewer with caret position

Enabling this in the plugin's menu (Plugins -> MarkdownPanel) makes the preview panel stay in sync with the caret in the markdown document that is being edited.  
This is similar to the _Synchronize Vertical Scrolling_ option of Notepad++ for keeping two open editing panels scrolling together.

### Synchronize with first visible line in editor

When this option is enabled, the plugin ensures that the first visible line in the 
editor is also visible in the preview. (This is an alternative to _Synchronize viewer with caret position_)

### Show Outline

When this option is enabled, an outline of the document is displayed. 
You can toggle the full outline with the burger menu switch.
The outline is disabled by default.
![/npp-outline.png](help/npp-outline.png "Outline")	

### Shortcut to toggle Panel
It is possible to toggle the Panel with a shortcut. Therefore the Notepad++ "Shortcut Mapper" can be used.
The Shortcut Mapper can be opened by "Settings" -> "Shortcut Mapper..." and switch to the tab "Plugin commands". 
Than bind a shortcut (eg. CTRL + SHIFT + M) to the command "Toggle Markdown Panel".
After that it's possible to use the shortcut to open or close the panel.
![shortcut.png](help/shortcut.png "Bind shortcut to toggle function")

## Version History

### Version 0.9.2 (released 2026-07-10)
- Features
	- Allow show/hide of source pane and not just rendered view pane [#138](https://github.com/mohzy83/NppMarkdownPanel/issues/138)) (implemented by letreset)
		-  see three state toggle
	- PDF Export [#153](https://github.com/mohzy83/NppMarkdownPanel/issues/153)) (implemented by letreset)
	- Disable wrapping text [#132](https://github.com/mohzy83/NppMarkdownPanel/issues/132))
		- min width of 250px introduced
	- Support of Mermaid [#108](https://github.com/mohzy83/NppMarkdownPanel/issues/108)) (implemented by letreset)
	- Display document outline [#167](https://github.com/mohzy83/NppMarkdownPanel/issues/167)) (implemented by letreset)
		- see "Show Outline"
	- Markdig updated to version 1.3.2
	- Defer WebView2 initialization (just initialize WebView2 when preview is actually shown) [#164](https://github.com/mohzy83/NppMarkdownPanel/issues/164))

- Bug fixes
	- Fixed: Local relative path not working with img tag [#84](https://github.com/mohzy83/NppMarkdownPanel/issues/84))
	- Fixed: Markdown panel black when loading a large table in a detail element [#158](https://github.com/mohzy83/NppMarkdownPanel/issues/158))
	- Fixed: Add mermaid graph markdown setting pre tag class [#115](https://github.com/mohzy83/NppMarkdownPanel/issues/115))

### Version 0.9.1 (released 2026-01-19)

- Features
	- Preserve preview scroll position when switching to another file #96
	- Support (preview) files without an extension #133
	- Copy to clipboard as formatted text / HTML #128
	- "Save As.." function that always uses the light theme #126
![/saveas_lighttheme](help/saveas_lighttheme.png "Save As Light Theme")	
	- Text Callouts #141

- Bug fixes
	- Fixed: Undocking the main (preview) window eventually freezes NPP++ #106
    - Fixed: Markdown preview is not visualized if file extension is changed with MD panel open #107
	- Fixed: CoreWebView2 uninitialized exception on startup with unsaved backup file #139
	- Fixed: Memory Leak in plugin. #144
	- Fixed: Table cells display as individual squares with spacing in Markdown Panel #136, (contributed by [BdR76](https://github.com/BdR76)) #142
	- Remove build dependency on .NET 3.5 tools (contributed by [rdipardo](https://github.com/rdipardo)) #145
	- Fix memory layout of ScNotificationHeader (contributed by [rdipardo](https://github.com/rdipardo)) #146


### Version 0.9.0 (released 2025-06-20)
- requirements
	- .NET 4.7.2 or higher 
- features
	- WebView2 Edge integration (see settings to switch between HTML Rendering Engines)
	- Linking to filesystem files #8 ( works only with WebView2 (Edge) )
	- Disable file extension check #111, #112, #99
- bug fixes
	- NppTbData.pszModuleName is not properly assigned when Markdown panel is docked (contributed by [rdipardo](https://github.com/rdipardo)) #90
	- The keyboard shortcuts are not working on the Markdown Panel #91 (works only with WebView2 (Edge))
	- Undocking the viewer from its default position crashes NotePad++ (contributed by [rdipardo](https://github.com/rdipardo)) #97, #106
	- Meta tags in generated html not closed #100
	- Links pointing to file with whitespace (%20) is incorrectly parsed (%2520) #101
	- Incorrect opening of URL with CR encoded character (%0D) #105 (works only with WebView2 (Edge))
	- Add mermaid graph markdown setting pre tag class #115
	
### Version 0.7.3 (released 2023-02-12)
- bug fixes
	- Settings file NppMarkdownPanel.ini isn't used anymore #78
	- Plugin release v0.7.2 searches help files in wrong directory #76
	
### Version 0.7.2 (released 2023-02-11)
- bug fixes
	- Display images with Url-encoded space character (%20) in the filename (contributed by [andrzejQ](https://github.com/andrzejQ) ) #39
- features
	- Plugin-Menu entry renamed to **MarkdownPanel**
	- Syntax highlighting is now controlled by CSS Styles. See `style.css` and `style-dark.css` after comment `/* Syntax Highlighting */` #71
	- Feature to preprocess markdown files before they are send to the converter. Furthermore it's possible to postprocess the generated html files (created by markdig). 
	To enable this feature it's necessary to configure pre/post-processor commands (can be any commandline program) in the config file `plugins/Config/NppMarkdownPanel.ini`.
	The placeholders `%inputfile%` and `%outputfile%` have to be set in the commandline and will be resolved at runtime (with temporary file names).
	An example C# commandline-project can be found under: `misc\PPExtensions\MdpPrePostprocessorTemplate.sln`
```
[Options]
PreProcessorExe=C:\temp\preprocessor.exe
PreProcessorArguments=%inputfile% %outputfile%
PostProcessorExe=C:\temp\preprocessor\postprocessor.exe
PostProcessorArguments=%inputfile% %outputfile%
```

### Version 0.7.1 (released 2022-12-27)

- bug fixes
	- Footnotes (links to footnotes) don't work #28
	- Code fences not rendered for unknown languages (contributed by [rdipardo](https://github.com/rdipardo)) #55
	- Errorhandling when libraries are missing #57
	- Zoom label does not update on Settings panel init (contributed by [vinsworldcom](https://github.com/vinsworldcom)) #58
	- Settings dialog should render only if visible (contributed by [vinsworldcom](https://github.com/vinsworldcom)) #66
- features
	- Synchronize with first visible line in editor #14
    - Select/follow active editor pane when using mulitple editors #20
	- YAML Frontmatter is rendered as code block #46
	- Status bar to preview URLs for links (contributed by [vinsworldcom](https://github.com/vinsworldcom)) #60
	- Save As toolbar button provides default directory and filename (contributed by [vinsworldcom](https://github.com/vinsworldcom)) #61
	- Menu includes Help to access README / menu item order improved (contributed by [vinsworldcom](https://github.com/vinsworldcom)) #64
	
### Version 0.7.0 (released 2022-12-09)

- dark mode support (_requires Notepad++ version 8.4.1 or later_)
- new markdig 0.30.4 integrated
- code/syntax highlighting
	- example C# code with highlighting:
![code-highlighting](help/code-highlighting.png "Example code highlighting")
- new zoom level range from 80 % to 800% (for 4K Displays)
- all html files are saved as utf-8 files
- restrict preview to files with a specific extension
- automatically open panel for supported files
- enhanced about dialog


### Version 0.6.2 (released 2022-06-02)
Bugfix release
- viewer was crashed by too large documents (more than 10000 bytes)

### Version 0.6.1 (released 2022-05-26)
- fix embedded images
- fix dark icon

### Version 0.6.0 (released 2022-05-26)

- plugin headers for npp updated
- darkmode icon
- fixed refresh bug for 64-bit version of plugin
- new zoom level range from 40 % to 400%
- save html
- images for help file now included

### Version 0.5.0
- change zoomlevel for the preview in settings dialog
- change css file for the markdown style
- the new settings are persistent
- open settings dialog: Plugins-> NppMarkdownPanel -> Edit Settings

### Version 0.4.0
- switched from CommonMark.Net to markdig rendering library

### Version 0.3.0
- synchronize viewer with caret position

### Version 0.2.0
- Initial release

### Used libs and resources

| Name                              | Version | Authors                             | Link                                                                                                                   |
|-----------------------------------|---------|-------------------------------------|------------------------------------------------------------------------------------------------------------------------|
| **Markdig**                       | 1.3.2      | xoofx                               | [https://github.com/xoofx/markdig](https://github.com/xoofx/markdig)                                             |
| **NotepadPlusPlusPluginPack.Net** | 0.95    	  | kbilsted                            | [https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net](https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net) |
| **WebView2 Edge** 				| 1.0.3650.58 | Microsoft                           | [https://developer.microsoft.com/de-de/microsoft-edge/webview2?form=MA13LH](https://developer.microsoft.com/de-de/microsoft-edge/webview2?form=MA13LH) |
| **ColorCode (Portable)**          | 1.0.3       | Bashir Souid and Richard Slater     | [https://github.com/RichardSlater/ColorCodePortable](https://github.com/RichardSlater/ColorCodePortable)               |
| **Markdig.SyntaxHighlighting**    | 1.1.7       | Richard Slater                      | [https://github.com/RichardSlater/Markdig.SyntaxHighlighting](https://github.com/RichardSlater/Markdig.SyntaxHighlighting) |
| **github-markdown-css**           | 3.0.1       | sindresorhus                        | [https://github.com/sindresorhus/github-markdown-css](https://github.com/sindresorhus/github-markdown-css)             |
| **Markdown icon**                 |             | dcurtis                             | [https://github.com/dcurtis/markdown-mark](https://github.com/dcurtis/markdown-mark)                                   |
| **markdown-it-github-alerts**     | 1.0.0       | antfu                               | [https://github.com/antfu/markdown-it-github-alerts](https://github.com/antfu/markdown-it-github-alerts)                                   |
| **ClipboardHelper (HTML Renderer)**	| 1.5.2       | Arthur Teplitzki                    | [https://github.com/ArthurHub/HTML-Renderer/blob/master/Source/HtmlRenderer.WinForms/Utilities/ClipboardHelper.cs](https://github.com/ArthurHub/HTML-Renderer/blob/master/Source/HtmlRenderer.WinForms/Utilities/ClipboardHelper.cs)                                   |


The plugin uses portions of nea's **MarkdownViewerPlusPlus** Plugin code - [https://github.com/nea/MarkdownViewerPlusPlus](https://github.com/nea/MarkdownViewerPlusPlus)



### Contributors

Thanks to the contributors: 
[letreset](https://github.com/letreset), 
[vinsworldcom](https://github.com/vinsworldcom), 
[rdipardo](https://github.com/rdipardo), 
[andrzejQ](https://github.com/andrzejQ),
[chcg](https://github.com/chcg),
[BdR76](https://github.com/BdR76),
[RicoP](https://github.com/RicoP),
[UrsineRaven](https://github.com/UrsineRaven)
 and
[eeucalyptus](https://github.com/eeucalyptus)

## License

This project is licensed under the MIT License - see the LICENSE.txt file for details
