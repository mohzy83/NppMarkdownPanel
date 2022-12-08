using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigWrapper
{
    public class Wrapper
    {
        private MarkdigMarkdownGenerator markdigMarkdownGenerator;
        public Wrapper()
        {
            markdigMarkdownGenerator = new MarkdigMarkdownGenerator();
        }

        public string ConvertToHtml(string markDownText, string filepath)
        {
            return markdigMarkdownGenerator.ConvertToHtml(markDownText, filepath);
        }
    }
}
