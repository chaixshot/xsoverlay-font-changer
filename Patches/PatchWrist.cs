using HarmonyLib;
using System.IO;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchWrist
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Wrist font patcher is loaded");
        }

        [HarmonyPatch("OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void PatchWebView(ref OverlayWebView wv)
        {
            if (Path.GetFileName(wv.LoadedURL) != "Wrist.html")
                return;

            Utils.ApplyHtmlStyle(wv, XConfig.WristPath.Value, ".clock-container, .media-widget-info-container, .performance-bar-text-name, .performance-bar-text-percentage");
        }
    }
}
