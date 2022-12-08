using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NppMarkdownPanel
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

        public MarkdigWrapperMarkdownGenerator()
        {
            var currentPluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var wrapperDllPath = Path.Combine(currentPluginPath, "lib", "MarkdigWrapper.dll");
            // References to other assemblies dont work in NPP ->
            // load Assembly using reflection from subdir npp/plugins/NppMarkdownPanel/lib/MarkdigWrapper.dll
            assembly = Assembly.LoadFrom(wrapperDllPath);
            type = assembly.GetType("MarkdigWrapper.Wrapper");
            wrapperInstance  = Activator.CreateInstance(type);
        }

        public string ConvertToHtml(string markDownText, string filepath)
        {
            object[] methodParams = { markDownText, filepath };
            object result = type.InvokeMember("ConvertToHtml",
                  BindingFlags.Default | BindingFlags.InvokeMethod,
                  null,
                  wrapperInstance,
                  methodParams);
            return result.ToString();
        }

    }
}
