using System.Text.RegularExpressions;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MarkdigWrapper.Markdig
{
    // Markdig defers emphasis resolution until the whole table has been read.
    // End the current cell's temporary emphasis container at its delimiter so
    // thousands of independent bold cells do not form one deeply nested chain.
    internal sealed class PipeTableEmphasisExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            // InlineProcessor releases the source lines before matching inlines.
            // Inspect the completed paragraph just before that happens.
            pipeline.BlockParsers.FindExact<ParagraphBlockParser>().Closed += (processor, block) =>
                block.ProcessInlinesBegin += (inlineProcessor, inline) =>
                    CellBoundaryPipeTableParser.MarkTable((LeafBlock)block);

            var original = pipeline.InlineParsers.FindExact<PipeTableParser>();
            if (original == null) return;
            var lineBreak = pipeline.InlineParsers.FindExact<LineBreakInlineParser>();
            pipeline.InlineParsers[pipeline.InlineParsers.IndexOf(original)] =
                new CellBoundaryPipeTableParser(lineBreak, original.Options);
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }

        private sealed class CellBoundaryPipeTableParser : PipeTableParser
        {
            private static readonly object IsTableKey = new object();
            private static readonly Regex Separator = new Regex(
                @"^\s*\|?\s*:?-+:?\s*(?:\|\s*:?-+:?\s*)+\|?\s*$");

            public CellBoundaryPipeTableParser(LineBreakInlineParser lineBreak, PipeTableOptions options)
                : base(lineBreak, options) { }

            public override bool Match(InlineProcessor processor, ref StringSlice slice)
            {
                var character = slice.CurrentChar;
                if (!base.Match(processor, ref slice)) return false;

                if ((character == '|' || character == '\n' || character == '\r') &&
                    processor.Block.ContainsData(IsTableKey) &&
                    processor.Root.LastChild is EmphasisDelimiterInline emphasis)
                {
                    emphasis.IsClosed = true;
                }
                return true;
            }

            public static void MarkTable(LeafBlock block)
            {
                bool result = block is ParagraphBlock && block.Lines.Count >= 2 &&
                    block.Lines.Lines[0].Slice.ToString().Contains("|") &&
                    Separator.IsMatch(block.Lines.Lines[1].Slice.ToString());
                if (!result) return;

                // A paragraph can start like a table and then continue as prose.
                // Leave that ambiguous case to the unmodified legacy parser.
                for (int i = 0; i < block.Lines.Count; i++)
                {
                    var line = block.Lines.Lines[i].Slice;
                    line.TrimStart();
                    if (line.CurrentChar != '|') return;
                }
                block.SetData(IsTableKey, true);
            }
        }
    }
}
