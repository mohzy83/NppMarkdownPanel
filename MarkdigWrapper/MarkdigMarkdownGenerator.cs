using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.SyntaxHighlighting;
using MarkdigWrapper.Markdig.YamlFrontMatter;
using Ganss.Xss;

namespace MarkdigWrapper
{
    public class MarkdigMarkdownGenerator
    {
        private static readonly HtmlSanitizer htmlSanitizer = new HtmlSanitizer();

        public MarkdigMarkdownGenerator()
        {
            htmlSanitizer.AllowedAttributes.Add("data-line");
            htmlSanitizer.AllowedAttributes.Add("class");
            htmlSanitizer.AllowedAttributes.Add("id");
            htmlSanitizer.AllowedSchemes.Add("file");
        }

        public string ConvertToHtml(string markDownText, string filepath, bool supportEscapeCharsInUris)
        {

            var sb = new StringBuilder();
            var htmlWriter = new StringWriter(sb);
            var htmlRenderer = new HtmlRenderer(htmlWriter);

            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                // YamlFrontMatter block needs to be parsed with UseYamlFrontMatter()
                // and is then rendered as code block with UseRenderYamlFrontMatterAsCodeBlock()
                .UseYamlFrontMatter()
                .UseRenderYamlFrontMatterAsCodeBlock()
                // Syntax Highlighting
                .UseSyntaxHighlighting()
                .UsePreciseSourceLocation();

            // Markdig's built-in AutoIdentifierExtension (bundled by UseAdvancedExtensions)
            // resolves duplicate heading ids by retrying "-1", "-2", "-3"... against a
            // HashSet until a free one is found, so the Nth heading sharing a base id costs
            // O(N) - making a document with many (duplicate or not) headings cost O(n^2)
            // overall. That turns documents with a few hundred+ headings into multi-second
            // (or worse) parses, which is what made large files appear to hang/not render.
            // AssignHeadingIdentifiers below replaces it with an O(n) equivalent that
            // produces identical ids (verified against real documents).
            pipelineBuilder.Extensions.RemoveAll(ext => ext is AutoIdentifierExtension);

            var pipeline = pipelineBuilder.Build();
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

            var result = "";

            try
            {
                var document = Markdown.Parse(markDownText, pipeline, null);

                AssignHeadingIdentifiers(document);
                SetLineNoAttributeOnAllBlocks(document);

                pipeline.Setup(htmlRenderer);
                htmlRenderer.Render(document);

                htmlWriter.Flush();
                result = sb.ToString();
                result = FixFragmentLinks(result);
                if (filepath != null) result = ResolveRelativePaths(result, filepath);
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            if (supportEscapeCharsInUris) result = UnescapeImageUris(result);
            if (supportEscapeCharsInUris) result = UnescapeAnchorUris(result);
            result = htmlSanitizer.Sanitize(result);
            return result;
        }

        // Equivalent to Markdig's built-in AutoIdentifierExtension (see the comment where
        // it's removed from the pipeline above), but O(n) instead of O(n^2): duplicate base
        // ids are disambiguated by a counter we already know is next, instead of retrying
        // "-1", "-2", "-3"... against a HashSet until one is free.
        private void AssignHeadingIdentifiers(MarkdownDocument document)
        {
            var usedIds = new Dictionary<string, int>();
            var stripWriter = new StringWriter();
            var stripRenderer = new HtmlRenderer(stripWriter) { EnableHtmlForInline = false };

            foreach (var node in document.Descendants())
            {
                if (!(node is HeadingBlock heading) || heading.Inline == null) continue;

                stripWriter.GetStringBuilder().Clear();
                stripRenderer.Render(heading.Inline);
                var plainText = System.Net.WebUtility.HtmlDecode(stripWriter.ToString());
                var baseId = LinkHelper.Urilize(plainText, true, false);

                string id;
                if (usedIds.TryGetValue(baseId, out int count))
                {
                    id = baseId + "-" + count;
                    usedIds[baseId] = count + 1;
                }
                else
                {
                    id = baseId;
                    usedIds[baseId] = 1;
                }

                var attributes = heading.GetAttributes();
                attributes.Id = id;
                heading.SetAttributes(attributes);
            }
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
                attributes.AddProperty("data-line", childBlock.Line.ToString());
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
            return regex.Replace(html, m =>
            {
                return Uri.UnescapeDataString(m.Value);
            });
        }

        private string UnescapeAnchorUris(string html)
        {
            // Unescape URI with % characters for non - US - ASCII characters in order to workaround
            // a bug under IE/Edge with local file links containing non US-ASCII chars.
            //               using System.Text.RegularExpressions;
            Regex regex = new Regex("href=\"file:///[^\"]+");
            return regex.Replace(html, m =>
            {
                return Uri.UnescapeDataString(m.Value);
            });
        }

        /// <summary>
        /// Markdig resolves fragment-only links (#anchor) against BaseUrl,
        /// producing file:///path/%23anchor. Convert them back to #anchor.
        /// </summary>
        private string FixFragmentLinks(string html)
        {
            var regex = new Regex("href=\"file:///[^\"]*%23([^\"]+)\"");
            return regex.Replace(html, m =>
            {
                return "href=\"#" + m.Groups[1].Value + "\"";
            });
        }

        private string ResolveRelativePaths(string html, string baseFilePath)
        {
            if (string.IsNullOrEmpty(baseFilePath)) return html;

            string baseDir;
            try
            {
                baseDir = Path.GetDirectoryName(baseFilePath);
                if (string.IsNullOrEmpty(baseDir)) return html;
            }
            catch
            {
                return html;
            }

            var regex = new Regex(
                @"(src|href)\s*=\s*[""']((?!\s*(?:https?|ftp|file|data|about|mailto|javascript):|//)[^""']+)[""']",
                RegexOptions.IgnoreCase
            );

            return regex.Replace(html, match =>
            {
                var attribute = match.Groups[1].Value;
                var relativePath = match.Groups[2].Value;

                if (string.IsNullOrWhiteSpace(relativePath) || relativePath.StartsWith("#"))
                    return match.Value;

                try
                {
                    var absolutePath = Path.GetFullPath(Path.Combine(baseDir, relativePath));
                    return attribute + "=\"file:///" + absolutePath.Replace('\\', '/') + "\"";
                }
                catch
                {
                    return match.Value;
                }
            });
        }
    }
}
