using System.IO;
using Vuplex.WebView;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches.WebView
{
    internal class Execute
    {
        public static void WaitForPageLoaded(OverlayWebView wv, string fontPath, int fontScale, string cssClass)
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

        public static void ApplyHtmlStyle(OverlayWebView wv, string fontPath, int fontScale, string cssClass)
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
                font-size: calc(100% + {3}px) !important;
            }}
        `;
        document.head.appendChild(style);

        return 'SUCCESS: Injected';
    }})();", htmlFile, GetFontType(htmlFile), cssClass, fontScale);

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
            string jsCode = $@"
    (function() {{
        if (!document.head) return 'ERROR: No Head';

        var removed = 0;

        // Remove <style> elements that contain our CustomFont or the provided css class
        var styles = Array.from(document.getElementsByTagName('style'));
        styles.forEach(function(s) {{
            try {{
                var txt = s.innerHTML || '';
                if (txt.indexOf('CustomFont') !== -1 || txt.indexOf('{cssClass}') !== -1 || txt.indexOf('@font-face') !== -1) {{
                    s.parentNode.removeChild(s);
                    removed++;
                }}
            }} catch(e) {{ /* ignore */ }}
        }});

        // Remove <link> elements that reference the injected font file or CustomFont (best-effort)
        var links = Array.from(document.getElementsByTagName('link'));
        links.forEach(function(l) {{
            try {{
                var href = l.href || '';

                if (href.indexOf('CustomFont') !== -1 || href.indexOf('{cssClass}') !== -1 || href.indexOf('.ttf') !== -1 || href.indexOf('.otf') !== -1) {{
                    l.parentNode.removeChild(l);
                    removed++;
                }}
            }} catch(e) {{ /* ignore */ }}
        }});

        // Also remove inline style attributes that force CustomFont on elements matching the class
        try {{
            if ('{cssClass}') {{
                var selector = '{cssClass}'.startsWith('.') || '{cssClass}'.startsWith('#') ? '{cssClass}' : '{cssClass}';
                var elems = document.querySelectorAll(selector);
                elems.forEach(function(el) {{
                    try {{
                        var fs = window.getComputedStyle(el).fontFamily || '';
                        if (fs.indexOf('CustomFont') !== -1) {{
                            el.style.fontFamily = '';
                            el.style.fontSize = '';
                            removed++;
                        }}
                    }} catch(e) {{ /* ignore */ }}
                }});
            }}
        }} catch(e) {{ /* ignore */ }}

        return 'SUCCESS: Removed ' + removed + ' style(s)';
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
