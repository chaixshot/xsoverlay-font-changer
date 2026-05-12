using HarmonyLib;
using System.IO;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchTooltip
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Tooltip font patcher is loaded");
        }

        [HarmonyPatch("OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void PatchWebView(ref OverlayWebView wv)
        {
            if (Path.GetFileName(wv.LoadedURL) != "Tooltip.html")
                return;

            Utils.ApplyHtmlStyle(wv, XConfig.TooltipPath.Value, ".tooltip-text");
        }
    }
}
