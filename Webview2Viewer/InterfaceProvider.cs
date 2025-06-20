using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webview2Viewer
{
    public class InterfaceProvider
    {
        public Webview2WebbrowserControl GetWebbrowserControl() {
            var webbrowserControl = new Webview2WebbrowserControl();
            return webbrowserControl;
        }
    }
}
