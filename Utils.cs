using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System.IO;
using Vuplex.WebView;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer
{
    internal class Utils
    {

        public static void ApplyHtmlStyle(OverlayWebView wv, string fontPath, string cssClass)
        {
            FileInfo fontFile = CopyFont(fontPath.Trim('"'));
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
                    Plugin.Logger.LogInfo($"Page loaded! Injecting {wv._webView.WebView.Url} CSS...");

                    wv._webView.WebView.ExecuteJavaScript(jsCode, (result) =>
                    {
                        Plugin.Logger.LogInfo($"[{Path.GetFileName(wv._webView.WebView.Url)}] {result}");
                    });
                }
            };
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
