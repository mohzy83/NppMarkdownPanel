using NppMarkdownPanel.Entities;
using System;

namespace NppMarkdownPanel.Forms
{
    public interface IViewerInterface
    {
        IntPtr Handle { get; }
        void InitRenderingEngine(Settings newSettings);
        void SetMarkdownFilePath(string filepath, bool isRename = false);
        void UpdateSettings(Settings settings);
        void RenderMarkdown(string currentText, string filepath, bool preserveVerticalScrollPosition = true);
        void ScrollToElementWithLineNo(int lineNo);
        bool IsValidFileExtension(string filename);
        void Cleanup();
        void ExportToPdf();
    }
}
