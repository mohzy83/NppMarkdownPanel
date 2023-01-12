using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppMarkdownPanel
{
    public static class Utils
    {
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
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

        /// <summary>
        /// Takes a filename or file path and returns whether or not it's valid.
        /// </summary>
        /// <param name="file">Filename or path of file</param>
        /// <param name="validFilePath">Set to the full path of file</param>
        /// <param name="errorText">If not valid, this is set to why</param>
        /// <param name="purpose">The purpose of the file being checked</param>
        /// <returns>Boolean representing whether or not file is valid</returns>
        public static bool ValidateFileSelection(string file, out string validFilePath, out string errorText, string purpose = "")
        {
            errorText = String.Empty;
            string pWithSpace = purpose;
            if (!String.IsNullOrWhiteSpace(purpose))
                pWithSpace = purpose.Trim() + " ";
            try
            {
                validFilePath = Path.GetFullPath(file);                             // Convert file name to the full path
                if (!IsDirectoryWritable(Path.GetDirectoryName(validFilePath)))     // Ensure that it's possible to write to chosen file
                    errorText = $"Can't save {pWithSpace}file to selected location!";
                FileInfo fi = null;
                try
                {
                    fi = new FileInfo(validFilePath);
                }
                catch (ArgumentException) { }
                catch (PathTooLongException) { }
                catch (NotSupportedException) { }
                if (fi is null || !String.IsNullOrEmpty(errorText))
                {
                    if (String.IsNullOrEmpty(errorText))
                        errorText = $"Invalid Path for {purpose}!";
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                errorText = $"Invalid Path for {purpose}!";
                validFilePath = null;
                return false;
            }
        }

        public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(
                    Path.Combine(
                        dirPath,
                        Path.GetRandomFileName()
                    ),
                    1,
                    FileOptions.DeleteOnClose)
                )
                { }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }

        /// <summary>
        /// Takes a fileName or fileName2 and returns it in validFileName whether file exists.
        /// </summary>
        /// <param name="fileName">Filename or path of file</param>
        /// <param name="fileName2">Filename or path of file</param>
        /// <param name="validFileName">File name of existing file</param>
        /// <returns>Boolean representing whether file with fileName or fileName2 exists</returns>
        public static bool FileNameExists(string fileName, string fileName2, out string validFileName)
        {
            validFileName = "";
            if (File.Exists(fileName))
            {
                validFileName = fileName;
            }
            else
            {
                if (File.Exists(fileName2))
                {
                    validFileName = fileName2;
                }
            }
            return !String.IsNullOrWhiteSpace(validFileName);
        }

    }
}
