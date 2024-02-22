using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace NppMarkdownPanel.Generator
{
    /*
     * This generator is using reflection to load markdig wrapper assembly at runtime and
     * to convert markdown to html (with markdig)
     * 
     * Unfortunately Notepad++ is not able load other assemblies from the plugin path at runtime
     * 
     */
    public class Webview2Panel
    {
        private Type webviewHosttype;
        private object webview2Host;
        private string errorText;

        public Webview2Panel(Control host)
        {
            AddWebview2(host);
        }


        private void AddWebview2(Control host)
        {
            try
            {
                var currentPluginPath = Utils.GetDirectoryOfExecutingAssembly();
                var wrapperDllPath2 = Path.Combine(currentPluginPath, "lib", "Webview2Viewer.dll");
                // References to other assemblies dont work in NPP ->
                // load Assembly using reflection from subdir npp/plugins/NppMarkdownPanel/lib/MarkdigWrapper.dll
                var myassembly = Assembly.LoadFrom(wrapperDllPath2);
                webviewHosttype = myassembly.GetType("Webview2Viewer.MDPWebview2Viewer");
                webview2Host = Activator.CreateInstance(webviewHosttype);

                object[] methodParams = { host };
                webviewHosttype.InvokeMember("AddViewerToHost",
                          BindingFlags.Default | BindingFlags.InvokeMethod,
                          null,
                          webview2Host,
                          methodParams);
            }
            catch (Exception e)
            {
            }
        }

        public void DisplayHtml(string html)
        {
            if (webview2Host != null)
            {
                try
                {
                    object[] methodParams2 = { html };
                    webviewHosttype.InvokeMember("SetHtml",
                              BindingFlags.Default | BindingFlags.InvokeMethod,
                              null,
                              webview2Host,
                              methodParams2);
                }
                catch (Exception e)
                {
                    // return string.Format("Error executing ConvertToHtml. Exception: {0}", e.Message);
                }
            }
            else
            {
                // return errorText;
            }
        }

    }
}
