using PanelCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigWrapper
{
    public class MarkdigWrapper : IMarkdownGenerator
    {
        private MarkdigMarkdownGenerator markdigMarkdownGenerator;
        public MarkdigWrapper()
        {
            markdigMarkdownGenerator = new MarkdigMarkdownGenerator();
        }

        public string ConvertToHtml(string markDownText, string filepath, bool supportEscapeCharsInUris)
        {
            return markdigMarkdownGenerator.ConvertToHtml(markDownText, filepath, supportEscapeCharsInUris);
        }
    }
}
