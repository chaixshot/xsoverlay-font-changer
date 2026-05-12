namespace xsoverlay_font_changer.Patches
{
    internal class PatchWindowSettingsFont
    {
        public static void PatchCSS()
        {
            Plugin.Logger.LogInfo($"Window Overlay Setting font patcher is loaded");

            Utils.ApplyHtmlStyle("WindowSettings", XConfig.WindowSettingsPath.Value, ".whitespace-pre");
        }
    }
}
