using Dbarone.Net.CommentarioServer;

namespace Dbarone.Net.CommentarioConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // process args
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
                    arguments.XmlCommentsPath,
                    arguments.ReadMePath);
                docGen.GenerateDocument();
            }
        }

        private static Arguments GetArguments(string[] args)
        {
            Arguments arguments = new Arguments();
            try
            {
                arguments = new Arguments(args);
            }
            catch
            {
                arguments = new Arguments();
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
    -c  --comments <xml comments path>          Path to optional comments.
    -r  --readme <readme path>                  Path to optional readme file.
    -t  --type <Html>                           Output type (currently only Html supported, and is default).

";
            Console.Write(help);
            Environment.Exit(0);
        }
    }
}
