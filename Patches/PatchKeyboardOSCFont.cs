using BepInEx.Bootstrap;
using WebSocketSharp;

namespace xsoverlay_font_changer.Patches
{
    // https://github.com/nyakowint/xsoverlay-keyboard-osc
    internal class PatchKeyboardOSCFont
    {
        public static void PatchCSS()
        {
            if (!IsInstalled())
                return;

            Plugin.configData.TryGetValue("KeyboardOSCFontPath", out string KeyboardOSCFontPath);

            if (KeyboardOSCFontPath.IsNullOrEmpty())
            {
                Plugin.Logger.LogError($"Config KeyboardOSCFontPath is missing. Fallback to default");
                return;
            }

            Utils.ApplyHtmlStyle("SettingsKO", KeyboardOSCFontPath, ".side-bar-button-text, .page-container, .page-header-text, .page-section-text, .whitespace-pre");
        }

        private static bool IsInstalled()
        {
            return Chainloader.PluginInfos.ContainsKey("nwnt.keyboardosc");
        }
    }
}
