// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
	public interface INotepadPPGateway
	{
		void FileNew();

		void AddToolbarIcon(int funcItemsIndex, toolbarIcons icon);
		void AddToolbarIcon(int funcItemsIndex, Bitmap icon);
		string GetNppPath();
		string GetPluginConfigPath();
		string GetCurrentFilePath();
		unsafe string GetFilePath(int bufferId);
		void SetCurrentLanguage(LangType language);
		bool OpenFile(string path);
	}

	/// <summary>
	/// This class holds helpers for sending messages defined in the Msgs_h.cs file. It is at the moment
	/// incomplete. Please help fill in the blanks.
	/// </summary>
	public class NotepadPPGateway : INotepadPPGateway
	{
		private const int Unused = 0;

		public void FileNew()
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_MENUCOMMAND, Unused, NppMenuCmd.IDM_FILE_NEW);
		}

		public void AddToolbarIcon(int funcItemsIndex, toolbarIcons icon)
		{
			IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(icon));
			try {
				Marshal.StructureToPtr(icon, pTbIcons, false);
				_ = Win32.SendMessage(
					PluginBase.nppData._nppHandle,
					(uint) NppMsg.NPPM_ADDTOOLBARICON,
					PluginBase._funcItems.Items[funcItemsIndex]._cmdID,
					pTbIcons);
			} finally {
				Marshal.FreeHGlobal(pTbIcons);
			}
		}

		public void AddToolbarIcon(int funcItemsIndex, Bitmap icon)
		{
			var tbi = new toolbarIcons();
			tbi.hToolbarBmp = icon.GetHbitmap();
			AddToolbarIcon(funcItemsIndex, tbi);
		}

		/// <summary>
		/// Gets the path of the current document.
		/// </summary>
		public string GetCurrentFilePath()
			=> GetString(NppMsg.NPPM_GETFULLCURRENTPATH);

		/// <summary>
		/// This method incapsulates a common pattern in the Notepad++ API: when
		/// you need to retrieve a string, you can first query the buffer size.
		/// This method queries the necessary buffer size, allocates the temporary
		/// memory, then returns the string retrieved through that buffer.
		/// </summary>
		/// <param name="message">Message ID of the data string to query.</param>
		/// <returns>String returned by Notepad++.</returns>
		public string GetString(NppMsg message)
		{
			int len = Win32.SendMessage(
					PluginBase.nppData._nppHandle,
					(uint) message, Unused, Unused).ToInt32()
				+ 1;
			var res = new StringBuilder(len);
			_ = Win32.SendMessage(
				PluginBase.nppData._nppHandle, (uint) message, len, res);
			return res.ToString();
		}

		/// <returns>The path to the Notepad++ executable.</returns>
		public string GetNppPath()
			=> GetString(NppMsg.NPPM_GETNPPDIRECTORY);

		/// <returns>The path to the Config folder for plugins.</returns>
		public string GetPluginConfigPath()
			=> GetString(NppMsg.NPPM_GETPLUGINSCONFIGDIR);

		/// <summary>
		/// Open a file for editing in Notepad++, pretty much like using the app's
		/// File - Open menu.
		/// </summary>
		/// <param name="path">The path to the file to open.</param>
		/// <returns>True on success.</returns>
		public bool OpenFile(string path)
			=> Win32.SendMessage(
				PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_DOOPEN, Unused, path).ToInt32()
			!= 0;

		/// <summary>
		/// Gets the path of the current document.
		/// </summary>
		public unsafe string GetFilePath(int bufferId)
		{
			var path = new StringBuilder(2000);
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_GETFULLPATHFROMBUFFERID, bufferId, path);
			return path.ToString();
		}

		public void SetCurrentLanguage(LangType language)
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_SETCURRENTLANGTYPE, Unused, (int) language);
		}
	}

	/// <summary>
	/// This class holds helpers for sending messages defined in the Resource_h.cs file. It is at the moment
	/// incomplete. Please help fill in the blanks.
	/// </summary>
	class NppResource
	{
		private const int Unused = 0;

		public void ClearIndicator()
		{
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) Resource.NPPM_INTERNAL_CLEARINDICATOR, Unused, Unused);
		}
	}
}
