using Kbg.NppPluginNET.PluginInfrastructure;

namespace NppMarkdownPanel
{
    class Main
    {
        public const string PluginName = "Markdown Panel";
        public const string PluginFilename = "MarkdownPanel";
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