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
                "NppMarkdownPanel for Notepad++\r\n\r\nVersion {0}\r\n\r\nCreated by Mohzy 2019-2023\r\n\r\nGithub: https://github.com/mohzy83/NppMarkdownPanel\r\n\r\nUsed Libs and Resources:\r\n\r\nMarkdig 0.30.4 by xoofx - https://github.com/lunet-io/markdig\r\n\r\nNotepadPlusPlusPluginPack.Net 0.95.00 by kbilsted - https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net\t\r\n\r\nColorCode (Portable) 1.0.3 by Bashir Souid and  Richard Slater\r\nhttps://github.com/RichardSlater/ColorCodePortable\r\n\r\nMarkdig.SyntaxHighlighting 1.1.7  - Syntax Highlighting extension for Markdig by Richard Slater\r\nhttps://github.com/RichardSlater/Markdig.SyntaxHighlighting\r\n\r\ngithub-markdown-css 3.0.1 by sindresorhus - \r\nhttps://github.com/sindresorhus/github-markdown-css\r\n\r\nMarkdown icon by dcurtis - https://github.com/dcurtis/markdown-mark\r\n\r\nThe plugin uses portions of nea's MarkdownViewerPlusPlus Plugin code - https://github.com/nea/MarkdownViewerPlusPlus";

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
