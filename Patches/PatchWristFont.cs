using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSocketSharp;

namespace xsoverlay_font_changer.Patches
{
    internal class PatchWristFont
    {
        public static async Task PatchAsync()
        {
            Plugin.configData.TryGetValue("WristFontPath", out string WristFontPath);

            if (WristFontPath.IsNullOrEmpty())
            {
                Plugin.Logger.LogError($"Config WristFontPath is missing. Fallback to default");
                return;
            }

            string htmlPath = ".\\XSOverlay_Data\\StreamingAssets\\Plugins\\Applications\\_UI\\Default\\Wrist.html";
            string html = File.ReadAllText(htmlPath);
            string fontName = "CustomFont";

            FileInfo fontFile = new(WristFontPath.Trim('"'));
            string destFile = $".\\XSOverlay_Data\\StreamingAssets\\Plugins\\Applications\\_UI\\Default\\_Shared\\fonts\\custom\\{fontFile.Name}";
            fontFile.CopyTo(destFile, true);
            fontFile = new(destFile);

            string htmlFile = fontFile.ToString().Replace(@"\XSOverlay_Data\StreamingAssets\Plugins\Applications\_UI\Default", "").Replace("\\", "/");
            string fontCss = $"\t\t@font-face {{\n\t\t\tfont-family: '{fontName}';\n\t\t\tsrc: url('{htmlFile}')\n\t\t\tformat('truetype');\n\t\t}}\n\t\tbody {{\n\t\t\tfont-family: '{fontName}';\n\t\t}}"; // The full CSS block

            if (html.Contains(fontName))
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
    }
}
