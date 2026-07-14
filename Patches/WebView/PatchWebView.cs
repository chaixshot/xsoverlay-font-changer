using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                    Dictionary<string, float> wristCssAdjustments = new()
                    {
                        { ".media-widget-track, .media-widget-artist, .performance-bar-text-name, .performance-bar-text-extra, .performance-bar-text-percentage", 0f },
                        { ".clock", 20f },
                        { ".date", 8f },
                        { ".time-in-vr", 2f }
                    };
                    RegisterPatch(_wv, XConfig.WristEnable, XConfig.WristName, XConfig.WristScale, wristCssAdjustments);
                    break;
                // Note: The wrist overlay has separate font size settings for the clock, date, and time-in-VR elements to ensure they remain legible and appropriately sized regardless of the overall wrist font scale.
                // This is necessary because these elements are often smaller and more detailed than other wrist UI components, so they require different scaling to maintain readability and visual consistency.

                case OverlayWebView.UserInterfacePaths.Settings:
                    Dictionary<string, float> settingsCssAdjustments = new()
                    {
                        { ".side-bar-button-text, .page-container, .page-header-text, .page-section-text, .whitespace-pre, .description-text, .button-font-text-basic, .option, .slider-tooltips", 0f }
                    };
                    RegisterPatch(_wv, XConfig.SettingsEnable, XConfig.SettingsName, XConfig.SettingsScale, settingsCssAdjustments);
                    break;

                case OverlayWebView.UserInterfacePaths.WindowSettings:
                    Dictionary<string, float> windowSettingsCssAdjustments = new()
                    {
                        { ".whitespace-pre, .option, .item-list-header, .item-list-appName, .item-list-title", 0f }
                    };
                    RegisterPatch(_wv, XConfig.WindowSettingsEnable, XConfig.WindowSettingsName, XConfig.WindowSettingsScale, windowSettingsCssAdjustments);
                    break;

                case OverlayWebView.UserInterfacePaths.Tooltip:
                    RegisterPatch(_wv, XConfig.TooltipEnable, XConfig.TooltipName, XConfig.TooltipScale, new Dictionary<string, float> { { ".tooltip-text", 0f } });
                    break;

                case OverlayWebView.UserInterfacePaths.Notification:
                    RegisterPatch(_wv, XConfig.NotificationEnable, XConfig.NotificationName, XConfig.NotificationScale, new Dictionary<string, float> { { "*", 0f } });
                    break;
            }
        }

        private static void RegisterPatch(OverlayWebView wv, ConfigEntry<bool> enable, ConfigEntry<string> path, ConfigEntry<float> scale, Dictionary<string, float> cssClass)
        {
            // Apply font style if enabled on initial load
            if (enable.Value)
            {
                foreach (KeyValuePair<string, float> entry in cssClass)
                {
                    Execute.WaitForPageLoaded(wv, path.Value, scale.Value + entry.Value, entry.Key);
                }
            }

            // Toggle enabling/disabling font style
            enable.SettingChanged += (sender, args) =>
            {
                if (enable.Value)
                {
                    foreach (KeyValuePair<string, float> entry in cssClass)
                    {
                        Execute.ApplyHtmlStyle(wv, path.Value, scale.Value + entry.Value, entry.Key);
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, float> entry in cssClass)
                    {
                        Execute.UndoHtmlStyle(wv, entry.Key);
                    }
                }
            };

            // Change font style
            path.SettingChanged += (sender, args) =>
            {
                if (enable.Value)
                {
                    foreach (KeyValuePair<string, float> entry in cssClass)
                    {
                        Execute.ApplyHtmlStyle(wv, path.Value, scale.Value + entry.Value, entry.Key);
                    }
                }
            };

            //Change font scale with debouncing to prevent excessive updates
            Coroutine debounceCoroutine = null;
            scale.SettingChanged += (sender, args) =>
            {
                if (enable.Value)
                {
                    if (debounceCoroutine != null) Plugin.Instance.StopCoroutine(debounceCoroutine);
                    debounceCoroutine = Plugin.Instance.StartCoroutine(ApplyWithDelay());
                }

                IEnumerator ApplyWithDelay()
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    foreach (KeyValuePair<string, float> entry in cssClass)
                    {
                        Execute.ApplyHtmlStyle(wv, path.Value, scale.Value + entry.Value, entry.Key);
                    }
                    debounceCoroutine = null;
                }
            };
        }
    }
}
