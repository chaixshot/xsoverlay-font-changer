using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Vuplex.WebView;
using XSOverlay;
using XSOverlay.WebApp;
using XSOverlay.Websockets.API;

namespace xsoverlay_font_changer.Patches.Setting
{
    internal class SettingPage
    {
        [Serializable]
        public class FontChangerSettings
        {
            public bool XSOverlayFontChanger_KeyboardEnable;
            public string XSOverlayFontChanger_KeyboardPath;
            public int XSOverlayFontChanger_KeyboardSize;

            public bool XSOverlayFontChanger_NotificationEnable;
            public string XSOverlayFontChanger_NotificationPath;
            public int XSOverlayFontChanger_NotificationSize;

            public bool XSOverlayFontChanger_SettingsEnable;
            public string XSOverlayFontChanger_SettingsPath;
            public int XSOverlayFontChanger_SettingsSize;

            public bool XSOverlayFontChanger_TooltipEnable;
            public string XSOverlayFontChanger_TooltipPath;
            public int XSOverlayFontChanger_TooltipSize;

            public bool XSOverlayFontChanger_WindowSettingsEnable;
            public string XSOverlayFontChanger_WindowSettingsPath;
            public int XSOverlayFontChanger_WindowSettingsSize;

            public bool XSOverlayFontChanger_WristEnable;
            public string XSOverlayFontChanger_WristPath;
            public int XSOverlayFontChanger_WristSize;
        }
        private static readonly System.Collections.Generic.Dictionary<int, string> fontList = [];
        private static readonly System.Collections.Generic.Dictionary<int, string> fontName = [];
        private static string fontListJS;

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

            FontChangerSettings settings = new()
            {
                // Keyboard
                XSOverlayFontChanger_KeyboardEnable = XConfig.KeyboardEnable.Value,
                XSOverlayFontChanger_KeyboardPath = XConfig.KeyboardPath.Value,
                XSOverlayFontChanger_KeyboardSize = XConfig.KeyboardScale.Value,

                // Notification
                XSOverlayFontChanger_NotificationEnable = XConfig.NotificationEnable.Value,
                XSOverlayFontChanger_NotificationPath = XConfig.NotificationPath.Value,
                XSOverlayFontChanger_NotificationSize = XConfig.NotificationScale.Value,

                // Settings
                XSOverlayFontChanger_SettingsEnable = XConfig.SettingsEnable.Value,
                XSOverlayFontChanger_SettingsPath = XConfig.SettingsPath.Value,
                XSOverlayFontChanger_SettingsSize = XConfig.SettingsScale.Value,

                // Tooltip
                XSOverlayFontChanger_TooltipEnable = XConfig.TooltipEnable.Value,
                XSOverlayFontChanger_TooltipPath = XConfig.TooltipPath.Value,
                XSOverlayFontChanger_TooltipSize = XConfig.TooltipScale.Value,

                // Window Settings
                XSOverlayFontChanger_WindowSettingsEnable = XConfig.WindowSettingsEnable.Value,
                XSOverlayFontChanger_WindowSettingsPath = XConfig.WindowSettingsPath.Value,
                XSOverlayFontChanger_WindowSettingsSize = XConfig.WindowSettingsScale.Value,

                // Wrist
                XSOverlayFontChanger_WristEnable = XConfig.WristEnable.Value,
                XSOverlayFontChanger_WristPath = XConfig.WristPath.Value,
                XSOverlayFontChanger_WristSize = XConfig.WristScale.Value
            };

            var data = JsonUtility.ToJson(settings);
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
                    XConfig.KeyboardPath.Value = fontList[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.KeyboardSize":
                    XConfig.KeyboardScale.Value = int.Parse(value);
                    break;

                // Notification
                case "XSOverlayFontChanger.NotificationEnable":
                    XConfig.NotificationEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.NotificationPath":
                    XConfig.NotificationPath.Value = fontList[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.NotificationSize":
                    XConfig.NotificationScale.Value = int.Parse(value);
                    break;

                // Settings
                case "XSOverlayFontChanger.SettingsEnable":
                    XConfig.SettingsEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.SettingsPath":
                    XConfig.SettingsPath.Value = fontList[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.SettingsSize":
                    XConfig.SettingsScale.Value = int.Parse(value);
                    break;

                // Tooltip
                case "XSOverlayFontChanger.TooltipEnable":
                    XConfig.TooltipEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.TooltipPath":
                    XConfig.TooltipPath.Value = fontList[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.TooltipSize":
                    XConfig.TooltipScale.Value = int.Parse(value);
                    break;

                // Window Settings
                case "XSOverlayFontChanger.WindowSettingsEnable":
                    XConfig.WindowSettingsEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.WindowSettingsPath":
                    XConfig.WindowSettingsPath.Value = fontList[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.WindowSettingsSize":
                    XConfig.WindowSettingsScale.Value = int.Parse(value);
                    break;

                // Wrist
                case "XSOverlayFontChanger.WristEnable":
                    XConfig.WristEnable.Value = bool.Parse(value);
                    break;
                case "XSOverlayFontChanger.WristPath":
                    XConfig.WristPath.Value = fontList[int.Parse(value)];
                    break;
                case "XSOverlayFontChanger.WristSize":
                    XConfig.WristScale.Value = int.Parse(value);
                    break;

                // About
                case "XSOverlayFontChanger.CheckForUpdate":
                    Utils.Update.CheckForUpdate();
                    break;
                case "XSOverlayFontChanger.OpenGitHub":
                    Utils.Update.OpenGitHubPage();
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
            jsContent = jsContent.Replace("<<FontList>>", fontListJS);

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
            var fontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
            var allowedExt = new System.Collections.Generic.HashSet<string>(new[] { ".ttf", ".otf", ".ttc", ".woff", ".woff2" }, System.StringComparer.OrdinalIgnoreCase);
            var seenFiles = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            var seenFamilies = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            var entries = new System.Collections.Generic.List<string>();
            int index = 0;

            // Read registry-installed fonts (canonical)
            try
            {
                using var regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts");
                if (regKey != null)
                {
                    foreach (var valName in regKey.GetValueNames())
                    {
                        var val = regKey.GetValue(valName) as string;
                        if (string.IsNullOrWhiteSpace(val))
                            continue;

                        var path = val;
                        if (!Path.IsPathRooted(path))
                            path = Path.Combine(fontsDir, path);

                        if (!File.Exists(path))
                            continue;

                        var ext = Path.GetExtension(path);
                        if (!allowedExt.Contains(ext))
                            continue;

                        var normalized = Path.GetFullPath(path).Replace('\\', '/');
                        if (!seenFiles.Add(normalized))
                            continue;

                        try
                        {
                            using var pfc = new System.Drawing.Text.PrivateFontCollection();
                            pfc.AddFontFile(normalized);

                            foreach (var fam in pfc.Families)
                            {
                                // Only include families that provide a Regular face
                                if (!fam.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                                    continue;

                                var familyName = fam.Name;
                                if (!seenFamilies.Add(familyName))
                                    continue;

                                var displayName = familyName;
                                var escaped = displayName.Replace("\\", "\\\\").Replace("'", "\\'");
                                entries.Add($"'{FormatKeyboardFontHtml(escaped)}'");

                                try { fontList[index] = normalized; } catch { fontList.Add(index, normalized); }
                                try { fontName[index] = displayName; } catch { fontName.Add(index, displayName); }
                                index++;
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
                foreach (var file in Directory.GetFiles(fontsDir))
                {
                    try
                    {
                        var ext = Path.GetExtension(file);
                        if (!allowedExt.Contains(ext))
                            continue;

                        var normalized = Path.GetFullPath(file).Replace('\\', '/');
                        if (!seenFiles.Add(normalized))
                            continue;

                        try
                        {
                            using var pfc = new System.Drawing.Text.PrivateFontCollection();
                            pfc.AddFontFile(normalized);

                            foreach (var fam in pfc.Families)
                            {
                                if (!fam.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                                    continue;

                                var familyName = fam.Name;
                                if (!seenFamilies.Add(familyName))
                                    continue;

                                var displayName = familyName;
                                var escaped = displayName.Replace("\\", "\\\\").Replace("'", "\\'");
                                entries.Add($"'{FormatKeyboardFontHtml(escaped)}'");

                                try { fontList[index] = normalized; } catch { fontList.Add(index, normalized); }
                                try { fontName[index] = displayName; } catch { fontName.Add(index, displayName); }
                                index++;
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
            fontListJS = string.Join(", ", entries);
        }

        private static string CleanRegistryFontName(string regName)
        {
            if (string.IsNullOrWhiteSpace(regName))
                return null;

            // Remove trailing parenthesis like " (TrueType)", keep primary name
            var idx = regName.IndexOf(" (", StringComparison.Ordinal);
            var name = idx > 0 ? regName.Substring(0, idx) : regName;
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
                using var pfc = new System.Drawing.Text.PrivateFontCollection();
                pfc.AddFontFile(filePath);
                var fams = pfc.Families;
                if (fams != null && fams.Length > 0)
                {
                    var name = fams[0].Name;
                    if (!string.IsNullOrWhiteSpace(name))
                        return name;
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

            var encoded = System.Net.WebUtility.HtmlEncode(displayName);
            return $"<font face=\"{encoded}\">{encoded}</font>";
        }
    }
}
