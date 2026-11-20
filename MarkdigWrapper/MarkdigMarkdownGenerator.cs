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
using Ganss.Xss;

namespace MarkdigWrapper
{
    public class MarkdigMarkdownGenerator
    {
        /// <summary>
        /// Disallowed tags whose content must never be shown. They are removed together with
        /// their children, while any other disallowed tag (e.g. a custom &lt;INSTRUCTIONS&gt; wrapper)
        /// is unwrapped so its content stays visible.
        /// </summary>
        private static readonly HashSet<string> tagsRemovedWithContent = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "script", "style", "template", "noscript", "noembed", "noframes", "xmp", "plaintext",
            "iframe", "frame", "frameset", "object", "embed", "applet", "param",
            "svg", "math", "head", "title", "meta", "link", "base"
        };

        private static readonly HtmlSanitizer htmlSanitizer = CreateHtmlSanitizer();

        private static HtmlSanitizer CreateHtmlSanitizer()
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("data-line");
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("id");
            sanitizer.AllowedSchemes.Add("file");
            // Unwrap unknown tags instead of deleting everything inside them (issue #194)
            sanitizer.KeepChildNodes = true;
            sanitizer.RemovingTag += (s, e) =>
            {
                if (tagsRemovedWithContent.Contains(e.Tag.LocalName))
                {
                    while (e.Tag.HasChildNodes) e.Tag.RemoveChild(e.Tag.FirstChild);
                }
            };
            return sanitizer;
        }

        public string ConvertToHtml(string markDownText, string filepath, bool supportEscapeCharsInUris)
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

            var result = "";

            try
            {
                var document = Markdown.Parse(markDownText, pipeline, null);

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
