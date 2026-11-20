using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MarkdigWrapper;

internal static class RendererRegressionTests
{
    private static readonly MarkdigMarkdownGenerator Generator = new MarkdigMarkdownGenerator();
    private static int checks;

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        checks++;
    }

    private static int Count(string html, string pattern)
    {
        return Regex.Matches(html, pattern).Count;
    }

    private static string Render(string markdown, bool forExport = false)
    {
        return Generator.ConvertToHtml(markdown,
            forExport ? null : @"C:\documents\test.md", !forExport);
    }

    private static int Main(string[] args)
    {
        try
        {
            string small = Render("| Item | Amount |\n| --- | ---: |\n| **bold** | 12.00 |\n");
            Assert(Count(small, "<table[ >]") == 1, "Small table missing.");
            Assert(small.Contains("<strong>bold</strong>"), "Small table lost bold formatting.");
            Assert(small.Contains("data-line=\"2\""), "Small table lost its source line.");

            // Rows wider than the header must retain their extra cells.
            string wider = Render("| A | B |\n| --- | --- |\n| one | two | three |\n");
            Assert(Count(wider, "<td[ >]") == 3, "Legacy support for wider table rows changed.");

            var table = new StringBuilder("| A | B | C | D | E |\n| --- | --- | --- | --- | --- |\n");
            for (int i = 0; i < 1500; i++)
                table.AppendFormat("| **row-{0}** | **two** | **three** | **four** | **five** |\n", i);
            string large = Render(table.ToString());
            Assert(Count(large, "<table[ >]") == 1, "Large table was not rendered as one table.");
            Assert(Count(large, "<tr[ >]") == 1501, "Large table lost rows.");
            Assert(Count(large, "<strong>") == 7500, "Large table lost bold formatting.");
            Assert(large.Contains("<strong>row-1499</strong>"), "Last large-table row missing.");
            Assert(large.Contains("<tr data-line=\"1501\">"), "Large table lost original source lines.");
            Assert(Count(Render(table.ToString(), true), "<tr[ >]") == 1501, "Export failed for large table.");

            // Exercise a wider row in a large table without dropping its extra cell.
            string largeWide = Render(table + "| **wide** | two | three | four | five | **retained extra cell** |\n");
            Assert(largeWide.Contains("<strong>retained extra cell</strong>"), "Large legacy table lost its extra column.");
            Assert(Count(largeWide, "<tr[ >]") == 1502, "Large legacy table lost its wider row.");

            string nestedEmphasis = Render("| A | B |\n| --- | --- |\n| **bold _italic_** | [**link**](https://example.com) |\n");
            Assert(nestedEmphasis.Contains("<strong>bold <em>italic</em></strong>"), "Nested emphasis changed inside a cell.");
            Assert(nestedEmphasis.Contains("<strong>link</strong></a>"), "Link emphasis changed inside a cell.");
            string paragraph = Render("**bold | across a literal pipe**\n");
            Assert(paragraph.Contains("<strong>bold | across a literal pipe</strong>"), "Non-table emphasis changed.");
            string continuation = Render("| A | B |\n| --- | --- |\n| **a** | **b** |\ntrailing **text**\n");
            Assert(!continuation.Contains("<table"), "Paragraph that resembles a table was changed into a table.");
            Assert(continuation.Contains("<strong>b</strong>"), "Paragraph continuation lost emphasis.");
            Assert(continuation.Contains("<strong>text</strong>"), "Paragraph continuation lost trailing emphasis.");

            string dangerous = table + "\n<script>alert(1)</script>\n\n<img src=\"x\" onerror=\"alert(1)\">\n";
            string sanitized = Render(dangerous);
            Assert(!sanitized.Contains("<script"), "Large-table rendering bypassed script sanitization.");
            Assert(!sanitized.Contains("onerror"), "Large-table rendering bypassed event-handler sanitization.");

            string details = Render("<details>\n<summary>Expand</summary>\n\n**Contents**\n\n</details>\n");
            Assert(details.Contains("<details>"), "Collapsible section missing.");
            Assert(details.Contains("<summary>Expand</summary>"), "Collapsible section summary missing.");
            Assert(details.Contains("<strong>Contents</strong>"), "Collapsible section contents missing.");

            string nested = Render(new string('>', 200) + " too deep\n");
            Assert(nested.Contains("depth limit exceeded"), "Actual nesting protection was disabled.");

            Console.WriteLine("PASS: " + checks + " assertions.");
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("FAIL: " + e.Message);
            return 1;
        }
    }
}
