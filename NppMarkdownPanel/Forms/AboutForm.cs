using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppMarkdownPanel.Forms
{
    public partial class AboutForm : Form
    {

        private const string AboutDialogText =
                "NppMarkdownPanel for Notepad++\r\n\r\nVersion {0}\r\n\r\nCreated by Mohzy 2019-2025\r\n\r\nGithub: https://github.com/mohzy83/NppMarkdownPanel\r\n\r\nUsed Libs and Resources:\r\n\r\n" +
                "Markdig 0.41.1 by xoofx - https://github.com/lunet-io/markdig\r\n\r\n" +
                "NotepadPlusPlusPluginPack.Net 0.95.00 by kbilsted - https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net\t\r\n\r\n" +
                "WebView2 Edge 1.0.3296.44 by Microsoft - https://developer.microsoft.com/de-de/microsoft-edge/webview2?form=MA13LH\r\n\r\n" +
                "ColorCode (Portable) 1.0.3 by Bashir Souid and  Richard Slater\r\nhttps://github.com/RichardSlater/ColorCodePortable\r\n\r\n" +
                "Markdig.SyntaxHighlighting 1.1.7  - Syntax Highlighting extension for Markdig by Richard Slater\r\nhttps://github.com/RichardSlater/Markdig.SyntaxHighlighting\r\n\r\n" +
                "github-markdown-css 3.0.1 by sindresorhus - \r\nhttps://github.com/sindresorhus/github-markdown-css\r\n\r\n" +
                "Markdown icon by dcurtis - https://github.com/dcurtis/markdown-mark\r\n\r\n" +
                "The plugin uses portions of nea's MarkdownViewerPlusPlus Plugin code - https://github.com/nea/MarkdownViewerPlusPlus";

        public AboutForm()
        {
            InitializeComponent();
            var versionString = "0.X";
            try
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                versionString = version.ToString();
            }
            catch (Exception) { }
            tbAbout.Text = string.Format(AboutDialogText, versionString);
            btnOk.Focus();
            this.ActiveControl = btnOk;
        }

    }
}
