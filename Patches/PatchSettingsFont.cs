using WebSocketSharp;

namespace xsoverlay_font_changer.Patches
{
    internal class PatchSettingsFont
    {
        public static void Patch()
        {
            Plugin.configData.TryGetValue("SettingFontPath", out string SettingFontPath);

            if (SettingFontPath.IsNullOrEmpty())
            {
                Plugin.Logger.LogError($"Config SettingFontPath is missing. Fallback to default");
                return;
            }

            Utils.ApplyHtmlStyle("Settings", SettingFontPath);
        }
    }
}
