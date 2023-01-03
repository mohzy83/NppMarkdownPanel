using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppMarkdownPanel.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            var versionString = "0.X";
            try
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                versionString = version.ToString();
            }
            catch (Exception) { }
            tbAbout.Text = string.Format(MainResources.AboutDialogText, versionString);
            btnOk.Focus();
            this.ActiveControl = btnOk;
        }

    }
}
