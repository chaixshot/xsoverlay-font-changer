using System.IO;
using System.Text.RegularExpressions;

namespace xsoverlay_font_changer
{
    internal class Utils
    {

        public static void ApplyHtmlStyle(string htmlName, string fontPath)
        {
            string htmlPath = $".\\XSOverlay_Data\\StreamingAssets\\Plugins\\Applications\\_UI\\Default\\{htmlName}.html";
            string html = File.ReadAllText(htmlPath);

            FileInfo fontFile = Copy(fontPath.Trim('"'));
            string htmlFile = fontFile.ToString().Replace(@"\XSOverlay_Data\StreamingAssets\Plugins\Applications\_UI\Default", "").Replace("\\", "/");
            string fontCss = $"\t\t@font-face {{\n\t\t\tfont-family: '{"CustomFont"}';\n\t\t\tsrc: url('{htmlFile}')\n\t\t\tformat('truetype');\n\t\t}}\n\t\t* {{\n\t\t\tfont-family: '{"CustomFont"}';\n\t\t}}"; // The full CSS block

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
    }
}
