using Kbg.NppPluginNET.PluginInfrastructure;

namespace NppMarkdownPanel
{
    class Main
    {
        // PluginName is used as npp plugin's menu entry
        public const string PluginName = "MarkdownPanel";
        // Modulename is used as config name (ini-file) and as _nppTbData.pszModuleName
        public const string ModuleName = "NppMarkdownPanel";
        public const string PluginTitle = "Markdown Panel";
        private static MarkdownPanelController mdpanel = new MarkdownPanelController();

        public static void OnNotification(ScNotification notification)
        {
            mdpanel.OnNotification(notification);
        }

        internal static void CommandMenuInit()
        {
            mdpanel.InitCommandMenu();
        }

        internal static void SetToolBarIcon()
        {
            mdpanel.SetToolBarIcon();
        }

        internal static void PluginCleanUp()
        {
            mdpanel.PluginCleanUp();
        }
    }
}