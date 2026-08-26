using System;
using System.IO;

namespace PanelCommon
{
    /// <summary>
    /// Shared virtual-host constants and local-path validation used by the
    /// Markdown preview and WebView2 host.
    /// </summary>
    public static class WebviewResourceConstants
    {
        public const string DocumentVirtualHostName = "markdownpanel-virtualhost";
        public const string DocumentVirtualBaseUrl =
            "http://" + DocumentVirtualHostName;

        public const string OfflineAssetsVirtualHostName =
            "markdownpanel-offline.local";
        public const string OfflineAssetsVirtualBaseUrl =
            "https://" + OfflineAssetsVirtualHostName + "/";

        public static string GetOfflineMermaidVirtualUrl(string localScriptPath)
        {
            string fileName = Path.GetFileName(localScriptPath);

            if (String.IsNullOrWhiteSpace(fileName))
                fileName = "mermaid.min.js";

            return OfflineAssetsVirtualBaseUrl + Uri.EscapeDataString(fileName);
        }

        /// <summary>
        /// Resolves a path to an existing file on a local drive.
        /// UNC paths, mapped network drives, relative paths, directories,
        /// invalid paths, and inaccessible files are rejected.
        /// </summary>
        public static bool TryGetExistingLocalFile(
            string path,
            out string fullPath)
        {
            fullPath = null;

            if (String.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path))
                return false;

            try
            {
                string candidate = Path.GetFullPath(path);

                if (IsNetworkPath(candidate) || !File.Exists(candidate))
                    return false;

                fullPath = candidate;
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                // Includes PathTooLongException.
                return false;
            }
        }

        /// <summary>
        /// Returns true for UNC paths and drive letters mapped to network shares.
        /// Invalid or inaccessible rooted paths are treated as unsafe.
        /// </summary>
        public static bool IsNetworkPath(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                if (path.StartsWith(@"\\", StringComparison.Ordinal) ||
                    path.StartsWith("//", StringComparison.Ordinal))
                {
                    return true;
                }

                if (!Path.IsPathRooted(path))
                    return false;

                string fullPath = Path.GetFullPath(path);
                string pathRoot = Path.GetPathRoot(fullPath);

                if (String.IsNullOrWhiteSpace(pathRoot))
                    return true;

                DriveInfo driveInfo = new DriveInfo(pathRoot);
                return driveInfo.DriveType == DriveType.Network;
            }
            catch (ArgumentException)
            {
                return true;
            }
            catch (NotSupportedException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            catch (IOException)
            {
                // Includes PathTooLongException.
                return true;
            }
        }
    }
}
