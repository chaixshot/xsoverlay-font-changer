﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Vuplex.WebView;
using XSOverlay;
using XSOverlay.WebApp;
using XSOverlay.Websockets.API;

namespace xsoverlay_font_changer.Patches.Setting
{
    internal class SettingPage
    {
        private static readonly Dictionary<int, string> FontNameById = [];
        private static string FontArrayJS;

        [HarmonyPatch(typeof(UpdateDateTime), "Awake")]
        [HarmonyPostfix]
        public static void InitializeEvents()
        {
            PrepairSystemFontList();
        }

        [HarmonyPatch(typeof(Overlay_Manager), "OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void WebviewOverlay(ref OverlayWebView wv)
        {
            if (wv.UserInterfaceSelection == OverlayWebView.UserInterfacePaths.Settings)
            {
                InjectSettingsModule(wv);
            }
        }

        [HarmonyPatch(typeof(ApiHandler), "OnRequestCurrentSettings")]
        [HarmonyPostfix]
        public static void OnRequestCurrentSettings(string sender)
        {
            if (!sender.Equals("systemui_settings")) return;

            Dictionary<string, object> settings = new()
            {
                // Keyboard
                ["XSOverlayFontChanger.KeyboardEnable"] = XConfig.KeyboardEnable.Value,
                ["XSOverlayFontChanger.KeyboardPath"] = XConfig.KeyboardPath.Value,
                ["XSOverlayFontChanger.KeyboardScale"] = XConfig.KeyboardScale.Value,

                // Notification
                ["XSOverlayFontChanger.NotificationEnable"] = XConfig.NotificationEnable.Value,
                ["XSOverlayFontChanger.NotificationPath"] = XConfig.NotificationPath.Value,
                ["XSOverlayFontChanger.NotificationScale"] = XConfig.NotificationScale.Value,

                // Settings
                ["XSOverlayFontChanger.SettingsEnable"] = XConfig.SettingsEnable.Value,
                ["XSOverlayFontChanger.SettingsPath"] = XConfig.SettingsPath.Value,
                ["XSOverlayFontChanger.SettingsScale"] = XConfig.SettingsScale.Value,

                // Tooltip
                ["XSOverlayFontChanger.TooltipEnable"] = XConfig.TooltipEnable.Value,
                ["XSOverlayFontChanger.TooltipPath"] = XConfig.TooltipPath.Value,
                ["XSOverlayFontChanger.TooltipScale"] = XConfig.TooltipScale.Value,

                // Window Settings
                ["XSOverlayFontChanger.WindowSettingsEnable"] = XConfig.WindowSettingsEnable.Value,
                ["XSOverlayFontChanger.WindowSettingsPath"] = XConfig.WindowSettingsPath.Value,
                ["XSOverlayFontChanger.WindowSettingsScale"] = XConfig.WindowSettingsScale.Value,

                // Wrist
                ["XSOverlayFontChanger.WristEnable"] = XConfig.WristEnable.Value,
                ["XSOverlayFontChanger.WristPath"] = XConfig.WristPath.Value,
                ["XSOverlayFontChanger.WristScale"] = XConfig.WristScale.Value,

                // About
                ["XSOverlayFontChanger.UpdateNotification"] = XConfig.UpdateNotification.Value,
            };

            string data = JsonUtility.ToJson(settings);
            ServerClientBridge.Instance.Api.SendMessage("UpdateSettings", data, null, sender);
        }


        [HarmonyPatch(typeof(XSettingsManager)), HarmonyPatch(nameof(XSettingsManager.SetSetting))]
        [HarmonyPrefix]
        public static bool SetSetting(string name, string value, string value1, bool sendAnalytics = true)
        {
            switch (name)
            {
                // Keyboard
                case "XSOverlayFontChanger.KeyboardEnable":
                    XConfig.KeyboardEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.KeyboardPath":
                    XConfig.KeyboardPath.Value = FontNameById[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.KeyboardScale":
                    XConfig.KeyboardScale.Value = float.Parse(value);
                    break;

                // Notification
                case "XSOverlayFontChanger.NotificationEnable":
                    XConfig.NotificationEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.NotificationPath":
                    XConfig.NotificationPath.Value = FontNameById[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.NotificationScale":
                    XConfig.NotificationScale.Value = float.Parse(value);
                    break;
                case "XSOverlayFontChanger.NotificationTestSmall":
                    NotificationHandler.Instance.CreateSmallTestNotification();
                    break;
                case "XSOverlayFontChanger.NotificationTestLarge":
                    NotificationHandler.Instance.CreateLargeTestNotification();
                    break;

                // Settings
                case "XSOverlayFontChanger.SettingsEnable":
                    XConfig.SettingsEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.SettingsPath":
                    XConfig.SettingsPath.Value = FontNameById[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.SettingsScale":
                    XConfig.SettingsScale.Value = float.Parse(value);
                    break;

                // Tooltip
                case "XSOverlayFontChanger.TooltipEnable":
                    XConfig.TooltipEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.TooltipPath":
                    XConfig.TooltipPath.Value = FontNameById[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.TooltipScale":
                    XConfig.TooltipScale.Value = float.Parse(value);
                    break;

                // Window Settings
                case "XSOverlayFontChanger.WindowSettingsEnable":
                    XConfig.WindowSettingsEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.WindowSettingsPath":
                    XConfig.WindowSettingsPath.Value = FontNameById[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.WindowSettingsScale":
                    XConfig.WindowSettingsScale.Value = float.Parse(value);
                    break;

                // Wrist
                case "XSOverlayFontChanger.WristEnable":
                    XConfig.WristEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.WristPath":
                    XConfig.WristPath.Value = FontNameById[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.WristScale":
                    XConfig.WristScale.Value = float.Parse(value);
                    break;

                // About
                case "XSOverlayFontChanger.CheckForUpdate":
                    Task.Run(Utils.Update.CheckForUpdate);
                    break;
                case "XSOverlayFontChanger.OpenGitHub":
                    Task.Run(Utils.Update.OpenGitHubPage);
                    break;
                case "XSOverlayFontChanger.UpdateNotification":
                    XConfig.UpdateNotification.Value = bool.Parse(value);
                    break;
            }

            return true;
        }

        private static void InjectSettingsModule(OverlayWebView wv)
        {
            // JS for inserting the actual settings page
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("xsoverlay_font_changer.Patches.Setting.setting.js");
            using StreamReader reader = new(stream);
            string jsContent = reader.ReadToEnd();

            jsContent = jsContent.Replace("<<KeyboardFont>>", FormatKeyboardFontHtml(GetFontDisplayName(XConfig.KeyboardPath.Value)));
            jsContent = jsContent.Replace("<<NotificationFont>>", FormatKeyboardFontHtml(GetFontDisplayName(XConfig.NotificationPath.Value)));
            jsContent = jsContent.Replace("<<SettingsFont>>", FormatKeyboardFontHtml(GetFontDisplayName(XConfig.SettingsPath.Value)));
            jsContent = jsContent.Replace("<<TooltipFont>>", FormatKeyboardFontHtml(GetFontDisplayName(XConfig.TooltipPath.Value)));
            jsContent = jsContent.Replace("<<WindowSettingsFont>>", FormatKeyboardFontHtml(GetFontDisplayName(XConfig.WindowSettingsPath.Value)));
            jsContent = jsContent.Replace("<<WristFont>>", FormatKeyboardFontHtml(GetFontDisplayName(XConfig.WristPath.Value)));
            jsContent = jsContent.Replace("<<FontList>>", FontArrayJS);
            jsContent = jsContent.Replace("<<Version>>", MyPluginInfo.PLUGIN_VERSION);

            string jsCode = $"(function() {{ {jsContent} }})();";

            // Lisen for WebView loaded
            wv._webView.WebView.LoadProgressChanged += (sender, args) =>
            {
                if (args.Type == ProgressChangeType.Finished)
                {
                    wv._webView.WebView.ExecuteJavaScript(jsCode, (result) =>
                    {
                        //Plugin.Logger.LogError($"[{wv.UserInterfaceSelection}] {result}");
                    });
                }
            };
        }

        private static void PrepairSystemFontList()
        {
            string fontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
            HashSet<string> allowedExt = new([".ttf", ".otf", ".ttc", ".woff", ".woff2"], StringComparer.OrdinalIgnoreCase);
            HashSet<string> seenFiles = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> seenFamilies = new(StringComparer.OrdinalIgnoreCase);
            List<string> entries = [];
            int index = 0;

            // Read registry-installed fonts (canonical)
            try
            {
                using (Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts"))
                if (regKey != null)
                {
                    foreach (string valName in regKey.GetValueNames())
                    {
                        string val = regKey.GetValue(valName) as string;
                        if (string.IsNullOrWhiteSpace(val))
                            continue;

                        string path = val;
                        if (!Path.IsPathRooted(path))
                            path = Path.Combine(fontsDir, path);

                        if (!File.Exists(path))
                            continue;

                        string ext = Path.GetExtension(path);
                        if (!allowedExt.Contains(ext))
                            continue;

                        string normalized = Path.GetFullPath(path).Replace('\\', '/');
                        if (!seenFiles.Add(normalized))
                            continue;

                        try
                        {
                            using (System.Drawing.Text.PrivateFontCollection pfc = new())
                            {
                            pfc.AddFontFile(normalized);

                            foreach (System.Drawing.FontFamily fam in pfc.Families)
                            {
                                // Only include families that provide a Regular face
                                if (!fam.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                                {
                                    continue;
                                }

                                string familyName = fam.Name;
                                if (!seenFamilies.Add(familyName))
                                {
                                    continue;
                                }

                                string displayName = familyName;
                                string escaped = displayName.Replace("\\", "\\\\").Replace("'", "\\'");
                                entries.Add($"'{FormatKeyboardFontHtml(escaped)}'");

                                try { FontNameById[index] = normalized; } catch { FontNameById.Add(index, normalized); }
                                index++;
                            }
                            }
                        }
                        catch
                        {
                            // ignore font load issues and continue
                        }
                    }
                }
            }
            catch
            {
                // ignore registry read issues and continue with folder scan
            }

            // Scan Fonts folder and add files not already seen
            if (Directory.Exists(fontsDir))
            {
                foreach (string file in Directory.GetFiles(fontsDir))
                {
                    try
                    {
                        string ext = Path.GetExtension(file);
                        if (!allowedExt.Contains(ext))
                            continue;

                        string normalized = Path.GetFullPath(file).Replace('\\', '/');
                        if (!seenFiles.Add(normalized))
                            continue;

                        try
                        {
                            using (System.Drawing.Text.PrivateFontCollection pfc = new())
                            {
                            pfc.AddFontFile(normalized);

                            foreach (System.Drawing.FontFamily fam in pfc.Families)
                            {
                                if (!fam.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                                    continue;

                                string familyName = fam.Name;
                                if (!seenFamilies.Add(familyName))
                                {
                                    continue;
                                }

                                string displayName = familyName;
                                string escaped = displayName.Replace("\\", "\\\\").Replace("'", "\\'");
                                entries.Add($"'{FormatKeyboardFontHtml(escaped)}'");

                                try { FontNameById[index] = normalized; } catch { FontNameById.Add(index, normalized); }
                                index++;
                            }
                            }
                        }
                        catch
                        {
                            // ignore per-file load errors
                        }
                    }
                    catch
                    {
                        // ignore per-file errors
                    }
                }
            }

            // Create final JS array string
            FontArrayJS = string.Join(", ", entries);
        }

        private static string CleanRegistryFontName(string regName)
        {
            if (string.IsNullOrWhiteSpace(regName))
                return null;

            // Remove trailing parenthesis like " (TrueType)", keep primary name
            int idx = regName.IndexOf(" (", StringComparison.Ordinal);
            string name = idx > 0 ? regName.Substring(0, idx) : regName;
            return name.Trim();
        }

        private static string GetFontDisplayName(string filePath)
        {
            string extension = Path.GetExtension(filePath);

            // For woff/woff2 fallback to filename (PrivateFontCollection doesn't support these)
            if (extension.Equals(".woff", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".woff2", StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFileNameWithoutExtension(filePath);
            }

            try
            {
                using (System.Drawing.Text.PrivateFontCollection pfc = new())
                {
                pfc.AddFontFile(filePath);
                System.Drawing.FontFamily[] fams = pfc.Families;
                if (fams != null && fams.Length > 0)
                {
                    string name = fams[0].Name;
                    if (!string.IsNullOrWhiteSpace(name))
                        return name;
                }
                }
            }
            catch
            {
                // ignore and fallback to filename
            }

            return Path.GetFileNameWithoutExtension(filePath);
        }

        private static string FormatKeyboardFontHtml(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return string.Empty;

            string encoded = System.Net.WebUtility.HtmlEncode(displayName);
            return $"<font face=\"{encoded}\">{encoded}</font>";
        }
    }
}
