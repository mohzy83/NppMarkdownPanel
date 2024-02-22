using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppMarkdownPanel.Webbrowser
{
    public interface IWebbrowserControl
    {
        void AddToHost(Control host);
        void PrepareContentUpdate(bool preserveVerticalScrollPosition);
        void SetContent(string content);
        void SetZoomLevel(int zoomLevel);
        void ScrollToElementWithLineNo(int lineNo);

        Bitmap MakeScreenshot();

        Action<string> StatusTextChangedAction { get; set; }
        Action RenderingDoneAction { get; set; }

    }
}
