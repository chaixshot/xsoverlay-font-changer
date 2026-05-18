using BepInEx.Configuration;
using HarmonyLib;
using System.Threading;
using System.Threading.Tasks;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches.WebView
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
            OverlayWebView _wv = wv;
            Plugin.Logger.LogInfo($"Patching {_wv.UserInterfaceSelection} WebView overlay...");

            switch (_wv.UserInterfaceSelection)
            {
                case OverlayWebView.UserInterfacePaths.URL:
                    break;

                case OverlayWebView.UserInterfacePaths.WindowToolbar:
                    break;

                case OverlayWebView.UserInterfacePaths.Wrist:
                    RegisterPatch(_wv, XConfig.WristEnable, XConfig.WristPath, XConfig.WristScale, ".media-widget-track, .media-widget-artist, .performance-bar-text-name, .performance-bar-text-extra, .performance-bar-text-percentage");
                    RegisterPatch(_wv, XConfig.WristEnable, XConfig.WristPath, XConfig.WristScale, ".clock");
                    RegisterPatch(_wv, XConfig.WristEnable, XConfig.WristPath, XConfig.WristScale, ".date");
                    RegisterPatch(_wv, XConfig.WristEnable, XConfig.WristPath, XConfig.WristScale, ".time-in-vr");
                    break;
                // Note: The wrist overlay has separate font size settings for the clock, date, and time-in-VR elements to ensure they remain legible and appropriately sized regardless of the overall wrist font scale.
                // This is necessary because these elements are often smaller and more detailed than other wrist UI components, so they require different scaling to maintain readability and visual consistency.
                //
                case OverlayWebView.UserInterfacePaths.Settings:
                    RegisterPatch(_wv, XConfig.SettingsEnable, XConfig.SettingsPath, XConfig.SettingsScale, ".side-bar-button-text, .page-container, .page-header-text, .page-section-text, .whitespace-pre, .description-text, .button-font-text-basic, .option, .slider-tooltips");
                    break;

                case OverlayWebView.UserInterfacePaths.WindowSettings:
                    RegisterPatch(_wv, XConfig.WindowSettingsEnable, XConfig.WindowSettingsPath, XConfig.WindowSettingsScale, ".whitespace-pre, .option, .item-list-header, .item-list-appName, .item-list-title");
                    break;

                case OverlayWebView.UserInterfacePaths.Tooltip:
                    RegisterPatch(_wv, XConfig.TooltipEnable, XConfig.TooltipPath, XConfig.TooltipScale, ".tooltip-text");
                    break;

                case OverlayWebView.UserInterfacePaths.Notification:
                    RegisterPatch(_wv, XConfig.NotificationEnable, XConfig.NotificationPath, XConfig.NotificationScale, "*");
                    break;
            }
        }

        private static void RegisterPatch(OverlayWebView wv, ConfigEntry<bool> enable, ConfigEntry<string> path, ConfigEntry<float> scale, string cssClasses)
        {
            // Get scale adjustment based on CSS class to ensure consistent visual size across different UI elements
            static float GetScaleAdjustment(string css) =>
                css switch
                {
                    ".clock" => 20,
                    ".date" => 8,
                    ".time-in-vr" => 2,
                    _ => 0
                };


            // Apply font style if enabled on initial load
            if (enable.Value)
                Execute.WaitForPageLoaded(wv, path.Value, scale.Value + GetScaleAdjustment(cssClasses), cssClasses);

            // Toggle enabling/disabling font style
            enable.SettingChanged += (s, e) =>
            {
                if (enable.Value)
                    Execute.ApplyHtmlStyle(wv, path.Value, scale.Value + GetScaleAdjustment(cssClasses), cssClasses);
                else
                    Execute.UndoHtmlStyle(wv, cssClasses);
            };

            // Change font style
            path.SettingChanged += (s, e) =>
            {
                if (enable.Value)
                {
                    if (GetScaleAdjustment(cssClasses).Equals(0))
                        Execute.UndoHtmlStyle(wv, cssClasses);
                    Execute.ApplyHtmlStyle(wv, path.Value, scale.Value + GetScaleAdjustment(cssClasses), cssClasses);
                }
            };

            //Change font scale with debouncing to prevent excessive updates
            CancellationTokenSource debounceCts = null;
            scale.SettingChanged += async (s, e) =>
            {
                if (enable.Value)
                {
                    debounceCts?.Cancel();
                    debounceCts?.Dispose();
                    debounceCts = new CancellationTokenSource();

                    await Task.Delay(500, debounceCts.Token);
                    Execute.ApplyHtmlStyle(wv, path.Value, scale.Value + GetScaleAdjustment(cssClasses), cssClasses);
                }
            };
        }
    }
}
