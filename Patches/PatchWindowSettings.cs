using HarmonyLib;
using System.IO;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchWindowSettings
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Window Overlay Settings font patcher is loaded");
        }

        [HarmonyPatch("OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void PatchWebView(ref OverlayWebView wv)
        {
            if (Path.GetFileName(wv.LoadedURL) != "WindowSettings.html")
                return;

            Utils.ApplyHtmlStyle(wv, XConfig.WindowSettingsPath.Value, ".whitespace-pre");
        }
    }
}
