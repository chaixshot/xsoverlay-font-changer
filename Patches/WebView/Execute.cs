using System.Collections.Generic;
using Vuplex.WebView;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches.WebView
{
    internal class Execute
    {
        private static readonly HashSet<string> copiedFonts = [];

        public static void WaitForPageLoaded(OverlayWebView wv, string fontName, float fontScale, string cssClass)
        {
            // Listen for WebView loaded
            void handler(object sender, ProgressChangedEventArgs args)
            {
                if (args.Type == ProgressChangeType.Finished)
                {
                    Plugin.Logger.LogInfo($"[{wv.UserInterfaceSelection}] Injecting \"{wv._webView.WebView.Url}\" CSS...");

                    wv._webView.WebView.LoadProgressChanged -= handler;
                    ApplyHtmlStyle(wv, fontName, fontScale, cssClass);
                }
            }

            wv._webView.WebView.LoadProgressChanged += handler;
        }

        public static void ApplyHtmlStyle(OverlayWebView wv, string fontName, float fontScale, string cssClass)
        {
            string styleId = GetStyleId(cssClass);

            string jsCode = string.Format(@"
    (function() {{
        if (!document.head) return 'ERROR: No Head';
        const id = '{0}';
        let style = document.getElementById(id);
        if (!style) {{
            style = document.createElement('style');
            style.id = id;
            document.head.appendChild(style);
        }}
        style.innerHTML = `
            {2} {{
                font-family: '{1}' !important;
                font-size: calc(100% + {3}px) !important;
            }}
        `;
        return 'SUCCESS: Applied ' + id;
    }})();", styleId, fontName, cssClass, fontScale);

            wv._webView.WebView.ExecuteJavaScript(jsCode, (result) =>
            {
                if (result.Contains("ERROR"))
                    Plugin.Logger.LogError($"[{wv.UserInterfaceSelection}] {result}");
                else
                    Plugin.Logger.LogInfo($"[{wv.UserInterfaceSelection}] {result}");
            });
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

        private static string GetStyleId(string cssClass)
        {
            return "xso-style-" + System.Math.Abs(cssClass.GetHashCode()).ToString("X");
        }
    }
}
