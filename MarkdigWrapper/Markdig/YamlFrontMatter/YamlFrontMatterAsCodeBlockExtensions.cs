using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigWrapper.Markdig.YamlFrontMatter
{
    public static class YamlFrontMatterAsCodeBlockExtensions
    {
        public static MarkdownPipelineBuilder UseRenderYamlFrontMatterAsCodeBlock(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Add(new YamlFrontMatterAsCodeBlockExtension());
            return pipeline;
        }
    }
}
