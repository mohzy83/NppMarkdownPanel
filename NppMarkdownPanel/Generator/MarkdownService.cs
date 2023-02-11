using NppMarkdownPanel.Generator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppMarkdownPanel.Generator
{
    public class MarkdownService
    {
        private const string INPUT_FILENAME_PLACEHOLDER = "%inputfile%";
        private const string OUTPUT_FILENAME_PLACEHOLDER = "%outputfile%";

        private IMarkdownGenerator markdownGenerator;

        public string PreProcessorCommandFilename { get; set; }
        public string PreProcessorArguments { get; set; }
        public string PostProcessorCommandFilename { get; set; }
        public string PostProcessorArguments { get; set; }

        public MarkdownService(IMarkdownGenerator markdownGenerator)
        {
            this.markdownGenerator = markdownGenerator;
        }

        public string ConvertToHtml(string markDownText, string filepath, bool supportEscapeCharsInImageUris)
        {
            var input = executeExternalProcessor(PreProcessorCommandFilename, PreProcessorArguments, markDownText);
            var html = markdownGenerator.ConvertToHtml(input, filepath, supportEscapeCharsInImageUris);
            return executeExternalProcessor(PostProcessorCommandFilename, PostProcessorArguments, html);
        }

        private string executeExternalProcessor(string commandFilename, string arguments, string input)
        {
            string result = input;
            if (!string.IsNullOrEmpty(commandFilename) && !string.IsNullOrEmpty(arguments))
            {
                var inputTempfilename = Path.GetTempFileName();
                var outputTempfilename = Path.GetTempFileName();
                try
                {
                    File.WriteAllText(inputTempfilename, input);
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = commandFilename;
                    string argumentsWithResolvedPlaceholders = arguments;
                    argumentsWithResolvedPlaceholders = argumentsWithResolvedPlaceholders.Replace(INPUT_FILENAME_PLACEHOLDER, "\"" + inputTempfilename + "\"");
                    argumentsWithResolvedPlaceholders = argumentsWithResolvedPlaceholders.Replace(OUTPUT_FILENAME_PLACEHOLDER, "\"" + outputTempfilename + "\"");
                    startInfo.Arguments = argumentsWithResolvedPlaceholders;
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    if (File.Exists(outputTempfilename))
                    {
                        var processedOutput = File.ReadAllText(outputTempfilename);
                        result = processedOutput;
                    }
                }
                catch (Exception e)
                {
                    result = string.Format("Error executing Pre/Postprocessor [{0}] with arguments [{1}] " + e.Message, commandFilename, arguments);
                }
                finally
                {
                    try
                    {
                        File.Delete(inputTempfilename);
                        File.Delete(outputTempfilename);
                    } catch (Exception)
                    {

                    }
                }

            }
            return result;
        }

    }
}
