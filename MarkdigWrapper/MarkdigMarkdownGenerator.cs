using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.SyntaxHighlighting;
using MarkdigWrapper.Markdig.YamlFrontMatter;

namespace MarkdigWrapper
{
    public class MarkdigMarkdownGenerator
    {

        public MarkdigMarkdownGenerator()
        {
        }

        public string ConvertToHtml(string markDownText, string filepath, bool supportEscapeCharsInImageUris)
        {
            var sb = new StringBuilder();
            var htmlWriter = new StringWriter(sb);
            var htmlRenderer = new HtmlRenderer(htmlWriter);

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                // YamlFrontMatter block needs to be parsed with UseYamlFrontMatter()
                // and is then rendered as code block with UseRenderYamlFrontMatterAsCodeBlock()
                .UseYamlFrontMatter()
                .UseRenderYamlFrontMatterAsCodeBlock()
                // Syntax Highlighting
                .UseSyntaxHighlighting()
                .UsePreciseSourceLocation()
                .Build();
            try
            {
                if (filepath != null)
                {
                    htmlRenderer.BaseUrl = new Uri(filepath);
                }
                else
                {
                    htmlRenderer.BaseUrl = null;
                }
            }
            catch (Exception e)
            {
                if (e != null) { }
            }
            sb.Clear();

            var document = Markdown.Parse(markDownText, pipeline, null);

            SetLineNoAttributeOnAllBlocks(document);

            pipeline.Setup(htmlRenderer);
            htmlRenderer.Render(document);

            htmlWriter.Flush();
            var result = sb.ToString();
            if (supportEscapeCharsInImageUris) result = UnescapeImageUris(result);
            return result;
        }

        private void SetLineNoAttributeOnAllBlocks(ContainerBlock rootBlock)
        {
            foreach (var childBlock in rootBlock)
            {
                if (childBlock is ContainerBlock)
                {
                    SetLineNoAttributeOnAllBlocks(childBlock as ContainerBlock);
                }
                var attributes = childBlock.GetAttributes();
                attributes.Id = childBlock.Line.ToString();
                childBlock.SetAttributes(attributes);
            }

        }

        private string UnescapeImageUris(string html)
        {
            // Unescape URI with % characters for non - US - ASCII characters in order to workaround
            // a bug under IE/Edge with local file links containing non US-ASCII chars.
            //               using System.Text.RegularExpressions;
            //string inp = " * %25%20x : `<img src=\"file:///C:/tmp/test%20nonAscii%20path/A%C4%85C%C4%87E/A%C4%84%2520(2).png\" />`";
            //outp:          * %25%20x : `<img src="file:///C:/tmp/test nonAscii path/AąCćE/AĄ%20(2).png" />`
            Regex regex = new Regex("src=\"file:///[^\"]+");
            return regex.Replace(html, m => {
                return Uri.UnescapeDataString(m.Value);
            });
        }
    }
}
