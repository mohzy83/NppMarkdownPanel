using ColorCode;
using ColorCode.Styling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigWrapper.Markdig.SyntaxHighlighting
{
    public class StyleToCssWriter
    {
        //Usage
        //StyleToCssWriter.WriteStylesToCssFile(styleSheet.Styles, @"C:\development\NppMarkdownPanel\out.css");

        private static string WriteColorToHex(Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static void WriteStylesToCssFile(StyleDictionary styles, string filePath)
        {
            string completeCss = "";

            foreach (var s in styles)
            {
                string cssClass = "";
                cssClass += "." + s.CssClassName + " {\n";
                if (s.Background != null)
                    cssClass += " background-color: " + WriteColorToHex(s.Background) + ";\n";
                if (s.Foreground != null)
                    cssClass += " color: " + WriteColorToHex(s.Foreground)+ ";\n";
                if (s.Bold)
                {
                    cssClass += " font-weight: bold;\n";
                }
                if (s.Italic)
                {
                    cssClass += " font-style: italic;\n";
                }
                cssClass += "}\n";

                completeCss += cssClass;

            }
            File.WriteAllText(filePath, completeCss);
        }
    }
}
