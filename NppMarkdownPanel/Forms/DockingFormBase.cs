using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;

namespace NppMarkdownPanel.Forms
{
    public partial class DockingFormBase : Form
    {
        private static Win32.WindowLongGetter _wndLongGetter;
        private static Win32.WindowLongSetter _wndLongSetter;

        public DockingFormBase()
        {
            InitializeComponent();
            if (Marshal.SizeOf(typeof(IntPtr)) == 8) // we are 64-bit
            {
                _wndLongGetter = Win32.GetWindowLongPtr;
                _wndLongSetter = Win32.SetWindowLongPtr;
            }
            else // we are 32-bit
            {
                _wndLongGetter = Win32.GetWindowLong;
                _wndLongSetter = Win32.SetWindowLong;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32.WM_NOTIFY:
                    var nmdr = (Win32.TagNMHDR)Marshal.PtrToStructure(m.LParam, typeof(Win32.TagNMHDR));
                    if (nmdr.hwndFrom == PluginBase.nppData._nppHandle)
                    {
                        switch ((DockMgrMsg)(nmdr.code & 0xFFFFU))
                        {
                            case DockMgrMsg.DMN_DOCK:   // we are being docked
                                break;
                            case DockMgrMsg.DMN_FLOAT:  // we are being _un_docked
                                RemoveControlParent(this);
                                break;
                            case DockMgrMsg.DMN_CLOSE:  // we are being closed
                                break;
                        }
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        private void RemoveControlParent(Control parent)
        {
            if (parent.HasChildren)
            {
                long extAttrs = (long)_wndLongGetter(parent.Handle, Win32.GWL_EXSTYLE);
                if (Win32.WS_EX_CONTROLPARENT == (extAttrs & Win32.WS_EX_CONTROLPARENT))
                {
                    _wndLongSetter(parent.Handle, Win32.GWL_EXSTYLE, new IntPtr(extAttrs & ~Win32.WS_EX_CONTROLPARENT));
                }
                foreach (Control c in parent.Controls)
                {
                    RemoveControlParent(c);
                }
            }
        }
    }
}
