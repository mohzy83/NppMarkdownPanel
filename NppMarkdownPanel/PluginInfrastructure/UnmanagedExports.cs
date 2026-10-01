// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using System;
using System.Runtime.InteropServices;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppMarkdownPanel;
using RGiesecke.DllExport;

namespace Kbg.NppPluginNET
{
    class UnmanagedExports
    {
        [DllExport(CallingConvention=CallingConvention.Cdecl)]
        static bool isUnicode()
        {
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static void setInfo(NppData notepadPlusData)
        {
            try
            {
                PluginBase.nppData = notepadPlusData;
                Main.CommandMenuInit();
            }
            catch
            {
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static IntPtr getFuncsArray(ref int nbF)
        {
            try
            {
                nbF = PluginBase._funcItems.Items.Count;
                return PluginBase._funcItems.NativePointer;
            }
            catch
            {
                nbF = 0;
                return IntPtr.Zero;
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static uint messageProc(uint Message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        static IntPtr _ptrPluginName = IntPtr.Zero;
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static IntPtr getName()
        {
            try
            {
                if (_ptrPluginName == IntPtr.Zero)
                    _ptrPluginName = Marshal.StringToHGlobalUni(Main.PluginName);
                return _ptrPluginName;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static void beNotified(IntPtr notifyCode)
        {
            try
            {
                ScNotification notification = (ScNotification)Marshal.PtrToStructure(notifyCode, typeof(ScNotification));
                if (notification.Header.Code == (uint)NppMsg.NPPN_TBMODIFICATION)
                {
                    PluginBase._funcItems.RefreshItems();
                    Main.SetToolBarIcon();
                }
                else if (notification.Header.Code == (uint)NppMsg.NPPN_SHUTDOWN)
                {
                    Main.PluginCleanUp();
                    if (_ptrPluginName != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(_ptrPluginName);
                        _ptrPluginName = IntPtr.Zero;
                    }
                }
                else
                {
                    Main.OnNotification(notification);
                }
            }
            catch
            {
            }
        }
    }
}
