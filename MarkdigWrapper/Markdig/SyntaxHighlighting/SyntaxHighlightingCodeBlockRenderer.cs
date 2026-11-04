using System;
using System.IO;
using System.Text;
using ColorCode;
using ColorCode.Formatting;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Markdig.SyntaxHighlighting
{
    public class SyntaxHighlightingCodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        private readonly CodeBlockRenderer _underlyingRenderer;
        private readonly IStyleSheet _customCss;

        public SyntaxHighlightingCodeBlockRenderer(CodeBlockRenderer underlyingRenderer = null, IStyleSheet customCss = null)
        {
            _underlyingRenderer = underlyingRenderer ?? new CodeBlockRenderer();
            _customCss = customCss;
        }

        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            var fencedCodeBlock = obj as FencedCodeBlock;
            var parser = obj.Parser as FencedCodeBlockParser;
            if (fencedCodeBlock == null || parser == null)
            {
                _underlyingRenderer.Write(renderer, obj);
                return;
            }

            var attributes = obj.TryGetAttributes() ?? new HtmlAttributes();

            var languageMoniker = fencedCodeBlock.Info.Replace(parser.InfoPrefix, string.Empty);

            if (string.IsNullOrEmpty(languageMoniker))
            {
                _underlyingRenderer.Write(renderer, obj);
                return;
            }

            // ColorCode's Markdown tokenizer matches fenced code blocks with a regex that
            // backtracks catastrophically:
            //     ^(```+[ \t]*\w*)((?:[ \t\r\n]|.)*?)(^```+)[ \t]*\r?$
            // A large ```markdown block (especially one containing nested fences) makes
            // Colorize() never return. Rendering runs on a background task, so the preview
            // silently keeps showing the previous document instead of the new one.
            // Render markdown blocks as plain code instead of highlighting them.
            if (IsMarkdownLanguage(languageMoniker))
            {
                _underlyingRenderer.Write(renderer, obj);
                return;
            }

            if (_underlyingRenderer != null
                && ((_underlyingRenderer.BlockMapping != null && _underlyingRenderer.BlockMapping.ContainsKey(languageMoniker))
                    || (_underlyingRenderer.BlocksAsDiv != null && _underlyingRenderer.BlocksAsDiv.Contains(languageMoniker))))
            {
                _underlyingRenderer.Write(renderer, obj);
                return;
            }

            attributes.AddClass($"lang-{languageMoniker}");
            attributes.Classes.Remove($"language-{languageMoniker}");

            attributes.AddClass("editor-colors");

            string firstLine;
            var code = GetCode(obj, out firstLine);

            renderer
                .Write("<div")
                .WriteAttributes(attributes)
                .Write(">");

            var markup = ApplySyntaxHighlighting(languageMoniker, firstLine, code);

            renderer.WriteLine(markup);
            renderer.WriteLine("</div>");
        }

        private static bool IsMarkdownLanguage(string languageMoniker)
        {
            var moniker = languageMoniker.Trim();
            return string.Equals(moniker, "markdown", StringComparison.OrdinalIgnoreCase)
                || string.Equals(moniker, "md", StringComparison.OrdinalIgnoreCase);
        }

        private string ApplySyntaxHighlighting(string languageMoniker, string firstLine, string code)
        {
            var languageTypeAdapter = new LanguageTypeAdapter();
            var language = languageTypeAdapter.Parse(languageMoniker, firstLine);

            if (language?.Id == null)
            { // TODO: handle unrecognised language formats, e.g. when using mermaid diagrams
                return code;
            }

            var codeBuilder = new StringBuilder();
            var codeWriter = new StringWriter(codeBuilder);
            var styleSheet = _customCss ?? StyleSheets.Default;
            var colourizer = new CodeColorizer();
            var htmlFormatter = new HtmlClassFormatter();
            colourizer.Colorize(code, language, htmlFormatter, styleSheet, codeWriter);
            return codeBuilder.ToString();
        }

        private static string GetCode(LeafBlock obj, out string firstLine)
        {
            var code = new StringBuilder();
            firstLine = null;
            foreach (var line in obj.Lines.Lines)
            {
                var slice = line.Slice;
                if (slice.Text == null)
                {
                    continue;
                }

                var lineText = slice.Text.Substring(slice.Start, slice.Length);

                if (firstLine == null)
                {
                    firstLine = lineText;
                }
                else
                {
                    code.AppendLine();
                }

                code.Append(lineText);
            }
            return code.ToString();
        }
    }
}
