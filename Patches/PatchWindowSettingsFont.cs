using WebSocketSharp;

namespace xsoverlay_font_changer.Patches
{
    internal class PatchWindowSettingsFont
    {
        public static void PatchCSS()
        {
            Plugin.configData.TryGetValue("WindowSettingsFontPath", out string WindowSettingsFontPath);

            if (WindowSettingsFontPath.IsNullOrEmpty())
            {
                Plugin.Logger.LogError($"Config WindowSettingsFontPath is missing. Fallback to default");
                return;
            }

            Utils.ApplyHtmlStyle("WindowSettings", WindowSettingsFontPath, ".whitespace-pre");
        }
    }
}
