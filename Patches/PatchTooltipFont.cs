namespace xsoverlay_font_changer.Patches
{
    internal class PatchTooltipFont
    {
        public static void PatchCSS()
        {
            Plugin.Logger.LogInfo($"Tooltip font patcher is loaded");

            Utils.ApplyHtmlStyle("Tooltip", XConfig.TooltipPath.Value, ".tooltip-text");
        }
    }
}
