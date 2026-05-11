using WebSocketSharp;

namespace xsoverlay_font_changer.Patches
{
    internal class PatchTooltipFont
    {
        public static void PatchCSS()
        {
            Plugin.configData.TryGetValue("TooltipFontPath", out string TooltipFontPath);

            if (TooltipFontPath.IsNullOrEmpty())
            {
                Plugin.Logger.LogError($"Config TooltipFontPath is missing. Fallback to default");
                return;
            }

            Utils.ApplyHtmlStyle("Tooltip", TooltipFontPath, ".tooltip-text");
        }
    }
}
