using MarkdownPanelStandalone.Properties;
using NppMarkdownPanel.Forms;
using NppMarkdownPanel.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NppMarkdownPanel;
using System.Web.Script.Serialization;
using System.Runtime;

namespace MarkdownPanelStandalone
{
    internal static class Program
    {
        private static IViewerInterface viewer;
        private static MarkdownPreviewForm form;

        const string CONFIG_FOLDER_NAME = "MarkdownPanel";
        const string CONFIG_FILE_NAME = "mdp-standalone.json";

        private static Settings settings = new Settings();
        const string APP_TITLE = "MarkdownPanel Standalone ";
        private static string currentFilePath;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            InitStandalone(args);
            Application.Run(form);
        }

        private static void InitStandalone(string[] args)
        {
            settings = LoadSettingsFormFile();
            viewer = (MarkdownPreviewForm)MarkdownPreviewForm.InitViewer(settings, null);
            form = (MarkdownPreviewForm)viewer;
            form.Icon = Resources.markdown_mark_solid_win10;
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            form.Text = APP_TITLE;
            form.Width = 1280;
            form.Height = 1000;

            AddOpenButton(form);
            AddSettingsButton(form);
            AddAboutButton(form);

            if (args.Length > 0)
            {
                var fn = args[0];
                currentFilePath = fn;
            }
            RenderCurrentMarkdownFile();
        }

        private static void RenderCurrentMarkdownFile()
        {
            if (!String.IsNullOrEmpty(currentFilePath))
            {
                viewer.SetMarkdownFilePath(currentFilePath);
                viewer.RenderMarkdown(File.ReadAllText(currentFilePath), currentFilePath);
                form.Text = APP_TITLE + " " + currentFilePath;
            }
            else
            {
                var fn = Path.Combine(Utils.GetDirectoryOfExecutingAssembly(), "home.md");
                viewer.SetMarkdownFilePath(fn);
                viewer.RenderMarkdown("## MarkdownPanel Standalone \n\n `Select Open File ...`", fn);
            }
        }

        private static void AddAboutButton(MarkdownPreviewForm form)
        {
            var tb = form.GetToolStrip();
            var btn = new System.Windows.Forms.ToolStripButton();
            btn.Name = "btnAboutDialog";
            btn.Text = "About";
            btn.Size = new System.Drawing.Size(93, 24);
            btn.Click += new EventHandler((o, e) =>
            {
                ShowAboutDialog();
            });
            tb.Items.Add(btn);
        }

        private static void ShowAboutDialog()
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private static void AddSettingsButton(MarkdownPreviewForm form)
        {
            var tb = form.GetToolStrip();
            var btn = new System.Windows.Forms.ToolStripButton();
            btn.Name = "btnSettingsDialog";
            btn.Text = "Settings";
            btn.Size = new System.Drawing.Size(93, 24);
            btn.Click += new EventHandler((o, e) =>
            {
                ShowSettingsDialog();
            });
            tb.Items.Add(btn);
        }

        private static void ShowSettingsDialog()
        {
            var settingsForm = new SettingsForm(settings);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                settings = settingsForm.GetDialogSettings(settings);
                settings.ShowToolbar = true;
                viewer.UpdateSettings(settings);
                SaveSettings(settings);
                RenderCurrentMarkdownFile();
            }
        }

        private static void AddOpenButton(MarkdownPreviewForm form)
        {
            var tb = form.GetToolStrip();
            var btn = new System.Windows.Forms.ToolStripButton();
            btn.Name = "btnOpen";
            btn.Text = "Open File";
            btn.Size = new System.Drawing.Size(93, 24);
            btn.Click += new EventHandler((o, e) =>
            {
                OpenFile();
            });
            tb.Items.Insert(0, btn);
        }

        private static void OpenFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = openFileDialog.FileName;
                    RenderCurrentMarkdownFile();
                }
            }
        }

        private static void SaveSettings(Settings settings)
        {
            string json = serializer.Serialize(settings);
            var settingsFile = GetConfigFilePath();
            File.WriteAllText(settingsFile, json);
        }

        private static string GetConfigFilePath()
        {
            var settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CONFIG_FOLDER_NAME);
            var settingsFile = Path.Combine(settingsDir, CONFIG_FILE_NAME);
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }
            return settingsFile;
        }

        private static Settings LoadSettingsFormFile()
        {
            var loadedSettings = new Settings();
            var settingsFile = GetConfigFilePath();
            if (File.Exists(settingsFile))
            {
                loadedSettings = serializer.Deserialize<Settings>(File.ReadAllText(settingsFile));
            }
            else
            {
                loadedSettings.ZoomLevel = 150;
                loadedSettings.SupportedFileExt = Settings.DEFAULT_SUPPORTED_FILE_EXT;
            }
            return loadedSettings;
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

    }
}
