using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Vuplex.WebView;
using XSOverlay;
using XSOverlay.WebApp;
using XSOverlay.Websockets.API;

namespace xsoverlay_font_changer
{
    internal class Utils
    {

        public static void ApplyHtmlStyle(OverlayWebView wv, string fontPath, string cssClass)
        {
            FileInfo fontFile = new(fontPath);

            if (fontFile.Exists)
            {
                fontFile = CopyFont(fontPath.Trim('"'));

                string htmlFile = fontFile.ToString().Replace(@"\XSOverlay_Data\StreamingAssets\Plugins\Applications\_UI\Default", "").Replace("\\", "/");
                string jsCode = string.Format(@"
    (function() {{
        if (!document.head) return 'ERROR: No Head';

        var style = document.createElement('style');
        style.innerHTML = `
            @font-face {{
                font-family: 'CustomFont';
                src: url('{0}') format('{1}'); 
            }}
            {2} {{
                font-family: 'CustomFont' !important;
            }}
        `;
        document.head.appendChild(style);

        return 'SUCCESS: Injected';
    }})();", htmlFile, GetFontType(htmlFile), cssClass);

                // Lisen for WebView loaded
                wv._webView.WebView.LoadProgressChanged += (sender, args) =>
                {
                    if (args.Type == ProgressChangeType.Finished)
                    {
                        Plugin.Logger.LogInfo($"[{wv.UserInterfaceSelection}] Page loaded!");
                        Plugin.Logger.LogInfo($"[{wv.UserInterfaceSelection}] Injecting \"{wv._webView.WebView.Url}\" CSS...");

                        wv._webView.WebView.ExecuteJavaScript(jsCode, (result) =>
                        {
                            if (result.Contains("ERROR"))
                                Plugin.Logger.LogError($"[{wv.UserInterfaceSelection}] {result}");
                            else
                                Plugin.Logger.LogInfo($"[{wv.UserInterfaceSelection}] {result}");
                        });
                    }
                };
            }
            else
            {
                Plugin.Logger.LogError($"{wv._overlay.overlayName} - \"{fontFile}\" does not exist.");
            }
        }

        private static FileInfo CopyFont(string fontPath)
        {
            string destDir = $".\\XSOverlay_Data\\StreamingAssets\\Plugins\\Applications\\_UI\\Default\\_Shared\\fonts\\custom";
            string destFile = $"{destDir}\\{Path.GetFileName(fontPath)}";

            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            if (!File.Exists(destFile))
                File.Copy(fontPath, destFile);

            return new(destFile);
        }

        private static string GetFontType(string fontPath)
        {
            return Path.GetExtension(fontPath) switch
            {
                ".ttf" => "TrueType",
                ".otf" => "OpenType",
                ".woff" => "Web Open Font Format",
                ".woff2" => "Web Open Font Format 2",
                _ => "TrueType",
            };
        }

        public static bool IsKeyboardOscInstalled()
        {
            return Chainloader.PluginInfos.ContainsKey("nwnt.keyboardosc");
        }

        public static async Task CheckVersion()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "xso-kbosc");

            Plugin.Logger.LogInfo("Checking for plugin updates...");
            try
            {
                var response = await client.GetStringAsync("https://raw.githubusercontent.com/chaixshot/xsoverlay-font-changer/main/VERSION");
                var remoteVersion = response.Trim();
                if (string.IsNullOrEmpty(remoteVersion))
                {
                    Plugin.Logger.LogError("VERSION file is empty or invalid.");
                    return;
                }

                if (remoteVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                    remoteVersion = remoteVersion.Substring(1);

                if (!Version.TryParse(remoteVersion, out var remoteVerObj))
                {
                    Plugin.Logger.LogError($"Failed to parse remote version string: '{remoteVersion}'");
                    return;
                }

                if (!Version.TryParse(MyPluginInfo.PLUGIN_VERSION, out var localVerObj))
                {
                    Plugin.Logger.LogWarning($"Local plugin version '{MyPluginInfo.PLUGIN_VERSION}' could not be parsed, skipping comparison.");
                    return;
                }

                Plugin.Logger.LogInfo($"Remote version: {remoteVerObj}, Local version: {localVerObj}");

                if (remoteVerObj > localVerObj)
                {
                    Plugin.Logger.LogInfo($"New version available! {remoteVerObj}");
                    ThreadingHelper.Instance.StartSyncInvoke(() => SendNotif("XSOverlay Font Changer update available!",
                        $"A new version of XSOverlay Font Changer [ {remoteVerObj} ] is available. You are currently using version {MyPluginInfo.PLUGIN_VERSION}."));
                }
                else
                {
                    Plugin.Logger.LogInfo("No updates available.");
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to check for updates: {ex.Message}");
            }
        }

        public static void SendNotif(string title, string content = "")
        {
            var notif = new Objects.NotificationObject
            {
                title = title,
                content = content,
                messageType = 1,
                timeout = 5f,
                height = CalculateHeight(content),
                sourceApp = "KeyboardOSC Plugin",
                volume = 0.5f
            };
            XSOEventSystem.Current.EventQueueNotification(notif);
        }

        private static int CalculateHeight(string content)
        {
            return content.Length switch
            {
                <= 100 => 100,
                <= 200 => 150,
                <= 300 => 200,
                _ => 250
            };
        }
    }

    internal class XConfig
    {
        public static ConfigEntry<string> KeyboardPath;
        public static ConfigEntry<bool> KeyboardEnable;

        public static ConfigEntry<string> NotificationPath;
        public static ConfigEntry<bool> NotificationEnable;

        public static ConfigEntry<string> SettingsPath;
        public static ConfigEntry<bool> SettingsEnable;

        public static ConfigEntry<string> TooltipPath;
        public static ConfigEntry<bool> TooltipEnable;

        public static ConfigEntry<string> WindowSettingsPath;
        public static ConfigEntry<bool> WindowSettingsEnable;

        public static ConfigEntry<string> WristPath;
        public static ConfigEntry<bool> WristEnable;

        public static void AllConfig(ConfigFile cfg)
        {
            KeyboardPath = cfg.Bind("Keyboard", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Keyboard font file.");
            KeyboardEnable = cfg.Bind("Keyboard", "Enable", true, "When enabled, Keyboard font will patch.");

            NotificationPath = cfg.Bind("Notification", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Notification font file.");
            NotificationEnable = cfg.Bind("Notification", "Enable", true, "When enabled, Notification font will patch.");

            SettingsPath = cfg.Bind("Settings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Settings font file.");
            SettingsEnable = cfg.Bind("Settings", "Enable", true, "When enabled, Settings font will patch.");

            TooltipPath = cfg.Bind("Tooltip", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Tooltip font file.");
            TooltipEnable = cfg.Bind("Tooltip", "Enable", true, "When enabled, Tooltip font will patch.");

            WindowSettingsPath = cfg.Bind("WindowSettings", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Window Overlay Settings font file.");
            WindowSettingsEnable = cfg.Bind("WindowSettings", "Enable", true, "When enabled, Window Overlay Settings font will patch.");

            WristPath = cfg.Bind("Wrist", "Path", "C:/WINDOWS/Fonts/segoeui.ttf", "Location of Wrist font file.");
            WristEnable = cfg.Bind("Wrist", "Enable", true, "When enabled, Wrist font will patch.");
        }
    }
}
