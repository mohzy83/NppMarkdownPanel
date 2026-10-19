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
        private static readonly HtmlSanitizer htmlSanitizer = new HtmlSanitizer();

        // Matches \(...\) and \[...\] math delimiters so they can be rewritten into the
        // $...$ / $$...$$ form before Markdig parses the text (Markdown would otherwise
        // strip the backslashes as escapes and the math could never be rendered).
        // Fenced code blocks and inline code spans are matched first and left untouched.
        private static readonly Regex latexDelimiterRegex = new Regex(
            @"(?<fence>^(?:```|~~~)[^\n]*\n(?:[\s\S]*?)(?:^(?:```|~~~)[^\n]*$|\z))" +
            @"|(?<code>`[^`\r\n]*`)" +
            @"|(?<disp>(?<!\\)\\\[(?<dcontent>[\s\S]*?)(?<!\\)\\\])" +
            @"|(?<inl>(?<!\\)\\\((?<icontent>[^\r\n]*?)(?<!\\)\\\))",
            RegexOptions.Multiline);

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
                markDownText = ConvertLatexMathDelimiters(markDownText);
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

        /// <summary>
        /// Rewrites \(...\) and \[...\] math delimiters into $...$ / $$...$$ so that the
        /// renderer's JavaScript sees them. Markdown would otherwise treat the backslashes
        /// as escapes and strip them. Fenced code blocks and inline code spans are kept
        /// verbatim; already-escaped delimiters (double backslash) are not touched.
        /// </summary>
        private static string ConvertLatexMathDelimiters(string markdown)
        {
            if (String.IsNullOrEmpty(markdown)) return markdown;
            if (markdown.IndexOf("\\(", StringComparison.Ordinal) < 0 &&
                markdown.IndexOf("\\[", StringComparison.Ordinal) < 0) return markdown;

            return latexDelimiterRegex.Replace(markdown, match =>
            {
                if (match.Groups["disp"].Success) return "$$" + match.Groups["dcontent"].Value + "$$";
                if (match.Groups["inl"].Success) return "$" + match.Groups["icontent"].Value + "$";
                return match.Value;
            });
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
