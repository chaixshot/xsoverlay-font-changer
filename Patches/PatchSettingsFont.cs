namespace xsoverlay_font_changer.Patches
{
    internal class PatchSettingsFont
    {
        public static void PatchCSS()
        {
            Plugin.Logger.LogInfo($"Settings font patcher is loaded");

            Utils.ApplyHtmlStyle("Settings", XConfig.SettingsPath.Value, ".side-bar-button-text, .page-container, .page-header-text, .page-section-text, .whitespace-pre");
        }
    }
}
