using System.Collections.Generic;
using System.IO;
using Vuplex.WebView;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches.WebView
{
    internal class Execute
    {
        private static readonly HashSet<string> copiedFonts = [];

        public static void WaitForPageLoaded(OverlayWebView wv, string fontPath, float fontScale, string cssClass)
        {
            // Listen for WebView loaded
            void handler(object sender, ProgressChangedEventArgs args)
            {
                if (args.Type == ProgressChangeType.Finished)
                {
                    Plugin.Logger.LogInfo($"[{wv.UserInterfaceSelection}] Injecting \"{wv._webView.WebView.Url}\" CSS...");

                    wv._webView.WebView.LoadProgressChanged -= handler;
                    ApplyHtmlStyle(wv, fontPath, fontScale, cssClass);
                }
            }

            wv._webView.WebView.LoadProgressChanged += handler;
        }

        public static void ApplyHtmlStyle(OverlayWebView wv, string fontPath, float fontScale, string cssClass)
        {
            FileInfo fontFile = new(fontPath);

            if (fontFile.Exists)
            {
                fontFile = CopyFont(fontPath.Trim('"'));

                string htmlFile = fontFile.ToString().Replace(@"\XSOverlay_Data\StreamingAssets\Plugins\Applications\_UI\Default", "").Replace("\\", "/");
                string styleId = GetStyleId(cssClass);

                string jsCode = string.Format(@"
    (function() {{
        if (!document.head) return 'ERROR: No Head';
        const id = '{4}';
        let style = document.getElementById(id);
        if (!style) {{
            style = document.createElement('style');
            style.id = id;
            document.head.appendChild(style);
        }}
        style.innerHTML = `
            @font-face {{
                font-family: 'CustomFont';
                src: url('{0}') format('{1}'); 
            }}
            {2} {{
                font-family: 'CustomFont' !important;
                font-size: calc(100% + {3}px) !important;
            }}
        `;
        return 'SUCCESS: Applied ' + id;
    }})();", htmlFile, GetFontType(htmlFile), cssClass, fontScale, styleId);

                wv._webView.WebView.ExecuteJavaScript(jsCode, (result) =>
                {
                    if (result.Contains("ERROR"))
                        Plugin.Logger.LogError($"[{wv.UserInterfaceSelection}] {result}");
                    else
                        Plugin.Logger.LogInfo($"[{wv.UserInterfaceSelection}] {result}");
                });
            }
            else
                Plugin.Logger.LogError($"{wv._overlay.overlayName} - \"{fontFile}\" does not exist.");
        }

        public static void UndoHtmlStyle(OverlayWebView wv, string cssClass)
        {
            string styleId = GetStyleId(cssClass);
            string jsCode = $@"
    (function() {{
        const style = document.getElementById('{styleId}');
        if (style) {{
            style.remove();
            return 'SUCCESS: Removed ' + '{styleId}';
        }}
        return 'SUCCESS: Not found';
    }})();";

            wv._webView.WebView.ExecuteJavaScript(jsCode, (result) =>
            {
                if (result.Contains("ERROR"))
                    Plugin.Logger.LogError($"[{wv.UserInterfaceSelection}] {result}");
                else
                    Plugin.Logger.LogInfo($"[{wv.UserInterfaceSelection}] {result}");
            });

            //wv._webView.WebView.Reload();
        }

        private static FileInfo CopyFont(string fontPath)
        {
            string destDir = $".\\XSOverlay_Data\\StreamingAssets\\Plugins\\Applications\\_UI\\Default\\_Shared\\fonts\\custom";
            string destFile = Path.Combine(destDir, Path.GetFileName(fontPath));

            if (copiedFonts.Contains(destFile))
                return new FileInfo(destFile);

            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            if (!File.Exists(destFile))
                File.Copy(fontPath, destFile);

            copiedFonts.Add(destFile);
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

        private static string GetStyleId(string cssClass)
        {
            return "xso-style-" + System.Math.Abs(cssClass.GetHashCode()).ToString("X");
        }
    }
}
