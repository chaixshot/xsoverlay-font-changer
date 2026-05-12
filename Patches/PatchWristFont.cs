namespace xsoverlay_font_changer.Patches
{
    internal class PatchWristFont
    {
        public static void PatchCSS()
        {
            Plugin.Logger.LogInfo($"Wrist font patcher is loaded");

            Utils.ApplyHtmlStyle("Wrist", XConfig.WristPath.Value, ".clock-container, .media-widget-info-container, .performance-bar-text-name, .performance-bar-text-percentage");
        }
    }
}
