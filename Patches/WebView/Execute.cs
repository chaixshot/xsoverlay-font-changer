using System.IO;
using Vuplex.WebView;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches.WebView
{
    internal class Execute
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
    }
}
