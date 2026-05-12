using HarmonyLib;
using System.IO;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchSettings
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Settings font patcher is loaded");
        }

        [HarmonyPatch("OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void PatchWebView(ref OverlayWebView wv)
        {
            if (Path.GetFileName(wv.LoadedURL) != "Settings.html")
                return;

            Utils.ApplyHtmlStyle(wv, XConfig.SettingsPath.Value, ".side-bar-button-text, .page-container, .page-header-text, .page-section-text, .whitespace-pre");
        }
    }
}
