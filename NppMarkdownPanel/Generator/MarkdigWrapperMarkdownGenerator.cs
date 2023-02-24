using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NppMarkdownPanel.Generator
{
    /*
     * This generator is using reflection to load markdig wrapper assembly at runtime and
     * to convert markdown to html (with markdig)
     * 
     * Unfortunately Notepad++ is not able load other assemblies from the plugin path at runtime
     * 
     */
    public class MarkdigWrapperMarkdownGenerator : IMarkdownGenerator
    {
        private Assembly assembly;
        private Type type;
        private Object wrapperInstance;
        private string errorText;

        public MarkdigWrapperMarkdownGenerator()
        {
            var wrapperDllPath = "";
            try
            {
                var currentPluginPath = Utils.GetDirectoryOfExecutingAssembly();
                wrapperDllPath = Path.Combine(currentPluginPath, "lib", "MarkdigWrapper.dll");
                // References to other assemblies dont work in NPP ->
                // load Assembly using reflection from subdir npp/plugins/NppMarkdownPanel/lib/MarkdigWrapper.dll
                assembly = Assembly.LoadFrom(wrapperDllPath);
                type = assembly.GetType("MarkdigWrapper.Wrapper");
                wrapperInstance = Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                errorText = string.Format("Error loading MarkdigWrapper from path {0}. Exception: {1}", wrapperDllPath, e.Message);
            }
        }

        public string ConvertToHtml(string markDownText, string filepath, bool supportEscapeCharsInImageUris)
        {
            if (wrapperInstance != null)
            {
                object[] methodParams = { markDownText, filepath, supportEscapeCharsInImageUris };

                try
                {
                    object result = type.InvokeMember("ConvertToHtml",
                          BindingFlags.Default | BindingFlags.InvokeMethod,
                          null,
                          wrapperInstance,
                          methodParams);
                    return result.ToString();

                }
                catch (Exception e)
                {
                    return string.Format("Error executing ConvertToHtml. Exception: {0}", e.Message);
                }
            }
            else
            {
                return errorText;
            }
        }

    }
}
