using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppMarkdownPanel.Entities
{
    public class RenderResult
    {
        public RenderResult(string resultForBrowser, string resultForExport)
        {
            ResultForBrowser = resultForBrowser;
            ResultForExport = resultForExport;
        }


        public string ResultForBrowser { get; set; }
        public string ResultForExport { get; set; }
    }
}
