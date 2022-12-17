using Markdig;
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
    public class YamlFrontMatterAsCodeBlockExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline) { }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer.ObjectRenderers.Contains<YamlFrontMatterHtmlRenderer>())
            {
                var yamlFrontMatterRenderer = renderer.ObjectRenderers.FindExact<YamlFrontMatterHtmlRenderer>();
                renderer.ObjectRenderers.Remove(yamlFrontMatterRenderer);
            }

            if (!renderer.ObjectRenderers.Contains<YamlFrontMatterAsCodeBlockHtmlRenderer>())
            {
                renderer.ObjectRenderers.InsertBefore<CodeBlockRenderer>(new YamlFrontMatterAsCodeBlockHtmlRenderer());
            }
        }
    }
}
