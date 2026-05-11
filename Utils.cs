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
}
