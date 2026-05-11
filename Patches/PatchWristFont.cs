using WebSocketSharp;

namespace xsoverlay_font_changer.Patches
{
    internal class PatchWristFont
    {
        public static void PatchCSS()
        {
            Plugin.configData.TryGetValue("WristFontPath", out string WristFontPath);

            if (WristFontPath.IsNullOrEmpty())
            {
                Plugin.Logger.LogError($"Config WristFontPath is missing. Fallback to default");
                return;
            }

            Utils.ApplyHtmlStyle("Wrist", WristFontPath, ".clock-container, .media-widget-info-container, .performance-bar-text-name, .performance-bar-text-percentage");
        }
    }
}
