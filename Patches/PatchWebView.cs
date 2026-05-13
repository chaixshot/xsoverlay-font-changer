using HarmonyLib;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchWebView
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            Plugin.Logger.LogInfo($"WebView Overlay font patcher is loaded");
        }

        [HarmonyPatch("OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void WebviewOverlay(ref OverlayWebView wv)
        {
            Plugin.Logger.LogInfo($"Patching {wv.UserInterfaceSelection} WebView overlay...");

            switch (wv.UserInterfaceSelection)
            {
                case OverlayWebView.UserInterfacePaths.URL:
                    break;
                case OverlayWebView.UserInterfacePaths.WindowToolbar:
                    break;
                case OverlayWebView.UserInterfacePaths.Wrist:
                    if (XConfig.WristEnable.Value)
                        Utils.ApplyHtmlStyle(wv, XConfig.WristPath.Value, ".clock-container, .media-widget-info-container, .performance-bar-text-name, .performance-bar-text-percentage");
                    break;
                case OverlayWebView.UserInterfacePaths.Settings:
                    if (XConfig.SettingsEnable.Value)
                        Utils.ApplyHtmlStyle(wv, XConfig.SettingsPath.Value, ".side-bar-button-text, .page-container, .page-header-text, .page-section-text, .whitespace-pre");
                    break;
                case OverlayWebView.UserInterfacePaths.WindowSettings:
                    if (XConfig.WindowSettingsEnable.Value)
                        Utils.ApplyHtmlStyle(wv, XConfig.WindowSettingsPath.Value, ".whitespace-pre");
                    break;
                case OverlayWebView.UserInterfacePaths.Tooltip:
                    if (XConfig.TooltipEnable.Value)
                        Utils.ApplyHtmlStyle(wv, XConfig.TooltipPath.Value, ".tooltip-text");
                    break;
                case OverlayWebView.UserInterfacePaths.Notification:
                    if (XConfig.NotificationEnable.Value)
                        Utils.ApplyHtmlStyle(wv, XConfig.NotificationPath.Value, ".notification-popup-title, .notification-popup-bodytext");
                    break;
            }
        }
    }
}
