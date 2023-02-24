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
    public delegate void ActionRef<T>(ref T item);

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
        /// Gets the directory path of the current executing assembly
        /// </summary>
        /// <returns></returns>
        public static string GetDirectoryOfExecutingAssembly()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

    }
}
