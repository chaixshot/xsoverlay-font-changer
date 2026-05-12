using HarmonyLib;
using System.IO;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchNotificationFont
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"Notification font patcher is loaded");
        }

        [HarmonyPatch("OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void PatchWebView(ref OverlayWebView wv)
        {
            if (Path.GetFileName(wv.LoadedURL) != "Notification.html")
                return;

            Utils.ApplyHtmlStyle(wv, XConfig.NotificationPath.Value, ".notification-popup-title, .notification-popup-bodytext");
        }
    }
}
