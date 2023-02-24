using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdpPrePostprocessorTemplate
{
    class Program
    {
        // Commandline Arguments
        // args[0] : Input file path
        // args[1] : Output file path
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                var inputFile = args[0];
                var ouputFile = args[1];
                if (File.Exists(inputFile))
                {
                    string fileContent = File.ReadAllText(inputFile);
                    // process file content
                    string output = fileContent.ToUpper();
                    File.WriteAllText(ouputFile, output);
                }
                else
                {
                    Console.WriteLine("Input file :" + inputFile + " is not present.");
                }
            }
            else
            {
                Console.WriteLine("Missing Paramters");
            }
        }
    }
}
