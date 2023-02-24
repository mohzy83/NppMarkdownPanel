using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NppMarkdownPanel
{
    public static class PluginUtils
    {

        /// <summary>
        /// Gets the directory path of the plugin (derived from current executing assembly)
        /// </summary>
        /// <returns></returns>
        public static string GetPluginDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// Parses and returns the provided ini entry as a boolean. If it fails, it returns fallback.
        /// </summary>
        /// <param name="section">The section of the ini file that the key is in</param>
        /// <param name="key">The key to read the value from</param>
        /// <param name="filename">The ini file to read</param>
        /// <param name="fallback">The value to return if it fails to parse the value as a bool</param>
        /// <returns>Boolean representation of the ini entry</returns>
        public static bool ReadIniBool(string section, string key, string filename, bool fallback = false)
        {
            try
            {
                return bool.TryParse(Win32.ReadIniValue(section, key, filename), out bool b) ? b : fallback;
            }
            catch (Exception)
            {
                return fallback;
            }
        }
    }
}
