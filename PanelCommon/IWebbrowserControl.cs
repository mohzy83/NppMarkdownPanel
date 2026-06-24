using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PanelCommon
{
    public interface IWebbrowserControl
    {
        void Initialize(int zoomLevel);
        void AddToHost(Control host);
        void PrepareContentUpdate(bool preserveVerticalScrollPosition);
        void SetContent(string content, string body, string style, string currentDocumentPath);
        void CurrentDocumentRenamed(string newDocumentPath);
        void SetZoomLevel(int zoomLevel);
        void ScrollToElementWithLineNo(int lineNo);
        string GetRenderingEngineName();

        Bitmap MakeScreenshot();

        Action<string> StatusTextChangedAction { get; set; }
        Action RenderingDoneAction { get; set; }
        Action AfterInitCompletedAction { get; set; }
        Action<int> CheckboxToggleAction { get; set; }
        Action<int> RadioToggleAction { get; set; }

        void Dispose();

        bool IsInitialized();

        void StopScrollPositionTracking();
    }
}
