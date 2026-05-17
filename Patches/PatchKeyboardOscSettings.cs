using HarmonyLib;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class PatchKeyboardOscSettings
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(Overlay_Manager __instance)
        {
            Plugin.Logger.LogInfo($"Keyboard OSC Settings font patcher is loaded");
        }

        [HarmonyPatch("OnRegisterWebviewOverlay")]
        [HarmonyPrefix]
        public static bool PatchCSS(ref OverlayWebView wv)
        {
            OverlayWebView _wv = wv;

            if (_wv.UserInterfaceSelection == OverlayWebView.UserInterfacePaths.Settings)
            {
                _wv._webView.WebView.UrlChanged += (sender, args) =>
                {
                    if (_wv._webView.WebView.Url.Contains("SettingsKO.html"))
                    {
                        Plugin.Logger.LogInfo("KeyboardOSC Replaced settings page url!");
                        WebView.Execute.ApplyHtmlStyle(_wv, XConfig.SettingsPath.Value, ".side-bar-button-text, .page-container, .page-header-text, .page-section-text, .whitespace-pre");
                    }
                };
            }

            return true;
        }
    }
}
