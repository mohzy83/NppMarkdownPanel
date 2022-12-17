using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigWrapper.Markdig.YamlFrontMatter
{
    public class YamlFrontMatterAsCodeBlockHtmlRenderer : HtmlObjectRenderer<YamlFrontMatterBlock>
    {
        private CodeBlockRenderer codeBlockRenderer;
        public YamlFrontMatterAsCodeBlockHtmlRenderer()
        {
            codeBlockRenderer = new CodeBlockRenderer();
        }
        protected override void Write(HtmlRenderer renderer, YamlFrontMatterBlock obj)
        {
            // Render Yaml Frontmatter as simple code block
            codeBlockRenderer.Write(renderer, obj);
        }
    }
}
