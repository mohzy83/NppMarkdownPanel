using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PanelCommon
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
