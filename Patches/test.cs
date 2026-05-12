using HarmonyLib;
using System.IO;
using Vuplex.WebView;
using XSOverlay;
using XSOverlay.WebApp;

namespace xsoverlay_font_changer.Patches
{
    [HarmonyPatch(typeof(Overlay_Manager))]
    internal class test
    {

        [HarmonyPatch("OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void Test(ref OverlayWebView wv)
        {
            Plugin.Logger.LogError($"{Path.GetFileName(wv.LoadedURL)}");


            var _wv = wv;
            _wv._webView.WebView.LoadProgressChanged += (sender, args) =>
            {
                Plugin.Logger.LogError($"{args.Type}");

                if (args.Type == ProgressChangeType.Finished)
                {
                    Plugin.Logger.LogInfo("Page loaded! Injecting CSS...");

                    string jsCode = string.Format(@"
    (function() {
        if (!document.head) {
            return 'ERROR: Document head not found yet!';
        }

        var style = document.createElement('style');
        style.innerHTML = '@font-face {{
            font-family: 'CustomFont';
            src: url('{0}') format('{1}');
        }}
        {2} {{
            font-family: 'CustomFont';
        }}';

        document.head.appendChild(style);

        return 'SUCCESS: Font rules injected into ' + document.location.href;
    })();
");

                    _wv._webView.WebView.ExecuteJavaScript(jsCode, (result) =>
                    {
                        Plugin.Logger.LogError($"{result}");
                    });
                }
            };
        }
    }
}
