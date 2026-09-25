using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppMarkdownPanel.Entities
{
    public class Settings
    {
        public const string DefaultCssFile = "style.css";
        public const string DefaultDarkModeCssFile = "style-dark.css";
        public const string DEFAULT_SUPPORTED_FILE_EXT = "md,mkd,mdwn,mdown,mdtxt,markdown,txt";

        public const string RENDERING_ENGINE_WEBVIEW1_IE11 = "IE11";
        public const string RENDERING_ENGINE_WEBVIEW2_EDGE = "EDGE";

        public Settings() { RenderingEngine = RENDERING_ENGINE_WEBVIEW2_EDGE; }

        public string CssFileName { get; set; }
        public string CssDarkModeFileName { get; set; }
        public int ZoomLevel { get; set; }
        public string HtmlFileName { get; set; }
        public string SupportedFileExt { get; set; }
        public bool SupportFilesWithNoExt { get; set; }
        public bool AllowAllExtensions { get; set; }        
        public bool IsDarkModeEnabled { get; set; }
        public bool ShowToolbar { get; set; }
        public bool ShowStatusbar { get; set; }
        public bool AutoShowPanel { get; set; }
        public bool EnableThreeStateToggle { get; set; }
        public bool ShowOutline { get; set; }

        // Use the editor colors of the active Notepad++ theme (Style Configurator) in the preview
        public bool UseNppThemeColors { get; set; }
        public Color EditorBackColor { get; set; }
        public Color EditorForeColor { get; set; }

        public bool HasEditorThemeColors()
        {
            return UseNppThemeColors && !EditorBackColor.IsEmpty && !EditorForeColor.IsEmpty;
        }

        /// <summary>
        /// Dark mode for the preview: follows the Notepad++ theme background if theme colors are used,
        /// otherwise the Notepad++ dark mode setting.
        /// </summary>
        public bool IsPreviewDark()
        {
            if (HasEditorThemeColors())
            {
                var luminance = 0.299 * EditorBackColor.R + 0.587 * EditorBackColor.G + 0.114 * EditorBackColor.B;
                return luminance < 128;
            }
            return IsDarkModeEnabled;
        }

        public string PreProcessorCommandFilename { get; set; }
        public string PreProcessorArguments { get; set; }
        public string PostProcessorCommandFilename { get; set; }
        public string PostProcessorArguments { get; set; }

        public string RenderingEngine { get; set; }

        public bool IsRenderingEngineIE11()
        {
            return RenderingEngine == RENDERING_ENGINE_WEBVIEW1_IE11;
        }

        public bool IsRenderingEngineEdge()
        {
            return RenderingEngine == RENDERING_ENGINE_WEBVIEW2_EDGE;
        }

    }
}
