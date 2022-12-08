﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace MarkdigWrapper
{
    public class MarkdigMarkdownGenerator 
    {
        private readonly HtmlRenderer htmlRenderer;
        private readonly StringWriter htmlWriter;
        private readonly StringBuilder sb;

        public MarkdigMarkdownGenerator()
        {
            sb = new StringBuilder();
            htmlWriter = new StringWriter(sb);
            htmlRenderer = new HtmlRenderer(htmlWriter);
        }

        public string ConvertToHtml(string markDownText, string filepath)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
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
                if (e != null) {}
            }
            sb.Clear();

            var document = Markdown.Parse(markDownText, pipeline, null);

            SetLineNoAttributeOnAllBlocks(document);

            pipeline.Setup(htmlRenderer);
            htmlRenderer.Render(document);

            htmlWriter.Flush();
            var result = sb.ToString();
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
    }
}
