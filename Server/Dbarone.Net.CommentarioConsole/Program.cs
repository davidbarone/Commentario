﻿using Dbarone.Net.CommentarioServer;

namespace Dbarone.Net.CommentarioConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // process args
            Console.Out.WriteLine("Starting Dbarone.Net.CommentarioConsole server...");
            Console.Out.WriteLine($"args[]: {args.ToString()}");

            var arguments = GetArguments(args);

            if (arguments.DisplayHelp)
            {
                DisplayHelp();
            }
            else
            {
                DocumentGenerator docGen = DocumentGenerator.Create(
                    arguments.AssemblyPath,
                    arguments.OutputPath,
                    arguments.OutputType,
                    arguments.AllowOverwrite,
                    arguments.XmlCommentsPath,
                    arguments.ReadMePath,
                    arguments.StylesPath);

                Console.Out.WriteLine("Starting document generation...");
                docGen.GenerateDocument();
                Console.Out.WriteLine("Completed document generation.");
            }
            Console.Out.WriteLine("Exiting Dbarone.Net.CommentarioConsole server...");
            Environment.Exit(0);
        }

        private static Arguments GetArguments(string[] args)
        {
            Console.Out.WriteLine("GetArguments: BEGIN...");

            Arguments arguments = new Arguments();
            try
            {
                arguments = new Arguments(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Out.WriteLine($"Warning - error getting arguments: {ex}");
                Console.Out.WriteLine($"Arguments being ignored...");
                Console.ResetColor();
                arguments = new Arguments();
            }
            finally
            {
                Console.Out.WriteLine(arguments.ToString());
                Console.Out.WriteLine("GetArguments: END...");
            }
            return arguments;
        }

        private static void DisplayHelp()
        {
            // Program class still available using 'scripting' format of Program.cs
            var help = @$"
{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}

Usage: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.exe [options][AssemblyPath][OutputPath]

Creates documentation from a .NET assembly and options xml comments file.

Options
    -h  --help                                  Displays this help.
    -d  --debug                                 Includes debug information in the output documentation. Default is off.
    -o  --overwrite                             Overwrites the output file if it already exists.
    -c  --comments <xml comments path>          Path to optional comments.
    -r  --readme <readme path>                  Path to optional readme file.
    -s  --styles <styles path>                  Path to optional styles file.
    -t  --type <Html>                           Output type (currently only Html supported, and is default).

";
            Console.Write(help);
        }
    }
}
