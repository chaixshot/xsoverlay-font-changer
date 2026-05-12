using BepInEx.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace xsoverlay_font_changer
{
    internal class Utils
    {

        public static void ApplyHtmlStyle(string htmlName, string fontPath, string cssClass)
        {
            fontPath = fontPath.Trim('"');
            string htmlPath = $".\\XSOverlay_Data\\StreamingAssets\\Plugins\\Applications\\_UI\\Default\\{htmlName}.html";
            string html = File.ReadAllText(htmlPath);

            FileInfo fontFile = Copy(fontPath);
            string htmlFile = fontFile.ToString().Replace(@"\XSOverlay_Data\StreamingAssets\Plugins\Applications\_UI\Default", "").Replace("\\", "/");
            string fontCss = string.Format(@"
        @font-face {{
            font-family: 'CustomFont';
            src: url('{0}') format('{1}');
        }}
        {2} {{
            font-family: 'CustomFont';
        }}", htmlFile, GetFontType(fontPath), cssClass);

            if (html.Contains("CustomFont"))
            {
                // If font exists, update the path inside url(...)
                html = Regex.Replace(html, @"url\('.*?\.ttf'\)", $"url('{htmlFile}')");
            }
            else
            {
                // If font doesn't exist, inject into <style> or create one
                if (html.Contains("<style>"))
                    html = html.Replace("<style>", $"<style>\n{fontCss}");
                else
                    html = html.Replace("</head>", $"\n\t<style>\n{fontCss}\n\t</style>\n</head>");
            }

            File.WriteAllText(htmlPath, html);
        }

        private static FileInfo Copy(string fontPath)
        {
            string destFile = $".\\XSOverlay_Data\\StreamingAssets\\Plugins\\Applications\\_UI\\Default\\_Shared\\fonts\\custom\\{Path.GetFileName(fontPath)}";

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
