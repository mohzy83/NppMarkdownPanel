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

        /// <summary>
        /// RegExp3replace
        /// </summary>
        /// <param name="inputStr">...</param>
        /// <param name="regExp3str">multiply 3-strings: Comment, Pattern, ReplacementPattern
        /// https://docs.microsoft.com/dotnet/standard/base-types/regular-expression-language-quick-reference
        /// </param>
        /// <returns>modified string</returns>
        public static string RegExp3replace(string inputStr, string[] regExp3lines)
        {
            if (regExp3lines.Length > 0)
            {
                string[] s123 = new String[3];
                for (int i = 0; i < regExp3lines.Length; i += 3)
                {
                    Array.Copy(regExp3lines, i, s123, 0, 3);

                    //https://regexone.com/references/csharp
                    //https://docs.microsoft.com/dotnet/standard/base-types/regular-expression-language-quick-reference
                    inputStr = System.Text.RegularExpressions.Regex.Replace(inputStr, s123[1], s123[2]);//comment in s123[0])

                }

            }
            return inputStr;
        }

        /// <summary>
        /// readRegExp3lines
        /// </summary>
        /// <param name="FinalRegExpFName">...</param>
        /// <returns>regExp3lines</returns>
        /// 

        public static string[] ReadRegExp3lines(string finalRegExpFName)
        {
            string regExp3str = File.ReadAllText(finalRegExpFName, Encoding.UTF8);//Utf8 with or w/o BOM 

            string[] regExp3lines = regExp3str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            if ((regExp3lines.Length % 3 != 0)
                && (regExp3lines[regExp3lines.Length - 1] == ""))
            { //remove last empty elem.
                Array.Resize(ref regExp3lines, regExp3lines.Length - 1);
            }
            int addSize = regExp3lines.Length % 3;
            if (addSize > 0)
            {
                addSize = 3 - addSize;
                Array.Resize(ref regExp3lines, regExp3lines.Length + addSize);
                while (addSize > 0)
                {
                    regExp3lines[regExp3lines.Length - addSize--] = "";
                }
            }
            for (int i = 2; i < regExp3lines.Length; i += 3)
            {
                regExp3lines[i] = regExp3lines[i]
                                    .Replace("\\n", "\n")
                                    .Replace("\\r", "\r")
                                    .Replace("\\t", "\t");
            }
            return regExp3lines;
        }








    }
}
