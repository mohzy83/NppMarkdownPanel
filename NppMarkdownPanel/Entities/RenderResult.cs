using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppMarkdownPanel.Entities
{
    public class RenderResult
    {
        public RenderResult(string resultForBrowser, string resultForExport, string resultBody, string resultStyle)
        {
            ResultForBrowser = resultForBrowser;
            ResultForExport = resultForExport;
            ResultBody = resultBody;
            ResultStyle = resultStyle;
        }


        public string ResultForBrowser { get; set; }
        public string ResultForExport { get; set; }
        public string ResultBody { get; set; }
        public string ResultStyle { get; set; }
    }
}
