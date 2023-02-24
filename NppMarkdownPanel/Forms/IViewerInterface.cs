using NppMarkdownPanel.Entities;
using System;

namespace NppMarkdownPanel.Forms
{
    public interface IViewerInterface
    {
        IntPtr Handle { get; }
        void SetMarkdownFilePath(string filepath);
        void UpdateSettings(Settings settings);
        void RenderMarkdown(string currentText, string filepath, bool preserveVerticalScrollPosition = true);
        void ScrollToElementWithLineNo(int lineNo);
        bool IsValidFileExtension(string filename);
    }
}
