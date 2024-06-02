using System.Text.Json;
using Dbarone.Net.CommentarioServer;

namespace Dbarone.Net.CommentarioConsole;

/// <summary>
/// Maps the command line arguments.
/// </summary>
public class Arguments
{
    public Arguments()
    {
        this.DisplayHelp = true;
    }

    /// <summary>
    /// https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html
    /// https://betterdev.blog/command-line-arguments-anatomy-explained/
    /// 
    /// Option / Flag - Can be single character (e.g. -h) or multiple character (e.g. --help)
    /// Some options / flags can be boolean, or can include an option-argument immediately after.
    /// At the end, positional arguments (operands) are generally mandatory.
    /// 
    /// format of args is:
    /// [options] [AssemblyPath] [OutputPath]
    /// 
    /// Options as follows:
    /// -h  --help                                  Displays this help.
    /// -d  --debug                                 Includes debug information in the output documentation. Default is off.
    /// -c  --comments &lt;xml comments path&gt;    Path to optional comments.
    /// -r  --readme &lt;readme path&gt;            Path to optional readme file.
    /// -s  --styles &lt;styles path&gt;            Path to optional styles file.
    /// -t  --type Html                             Output type (currently only Html supported, and is default).
    /// -o  --overwrite                             Allows the output file to be overwritten if it exists.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <exception cref="Exception">Throws an exception if invalid arguments passed.</exception>
    public Arguments(string[] args)
    {
        int i = 0;
        while (i < args.Length)
        {
            switch (args[i])
            {
                case "-h":
                case "--help":
                    this.DisplayHelp = true;
                    break;
                case "-d":
                case "--debug":
                    this.DebugMode = true;
                    break;
                case "-o":
                case "--overwrite":
                    this.AllowOverwrite = true;
                    break;
                case "-c":
                case "--comments":
                    i++;
                    if (i >= args.Length)
                    {
                        throw new Exception("Invalid arguments.");
                    }
                    this.XmlCommentsPath = args[i];
                    break;
                case "-r":
                case "--readme":
                    i++;
                    if (i >= args.Length)
                    {
                        throw new Exception("Invalid arguments.");
                    }
                    this.ReadMePath = args[i];
                    break;
                case "-s":
                case "--styles":
                    i++;
                    if (i >= args.Length)
                    {
                        throw new Exception("Invalid arguments.");
                    }
                    this.StylesPath = args[i];
                    break;
                case "-t":
                case "--type":
                    i++;
                    if (i >= args.Length)
                    {
                        throw new Exception("Invalid arguments.");
                    }
                    OutputType o = OutputType.html;
                    var result = Enum.TryParse<OutputType>(args[i], out o);
                    if (result)
                    {
                        this.OutputType = o;
                    }
                    else
                    {
                        throw new Exception("Invalid arguments.");
                    }
                    break;
                default:
                    // otherwise, is a positional argument / operand. There are only 2 possibilities.
                    if (this.AssemblyPath == default!)
                    {
                        this.AssemblyPath = args[i];
                    }
                    else if (this.OutputPath == default!)
                    {
                        this.OutputPath = args[i];
                    }
                    else
                    {
                        throw new Exception("Invalid arguments.");
                    }
                    break;
            }
            i++;
        }
    }

    /// <summary>
    /// When set to true, display the help.
    /// </summary>
    public Boolean DisplayHelp { get; set; }

    /// <summary>
    /// When set to true, Commentario will include additional debug content in the documentation to highlight missing commentary.
    /// </summary>
    public Boolean DebugMode { get; set; }

    /// <summary>
    /// The path to the .NET assembly that you wish to document.
    /// </summary>
    public string AssemblyPath { get; set; } = default!;

    /// <summary>
    /// The full path and file name of the output documentation. Needs to include the path, file and extension in the name.
    /// </summary>
    public string OutputPath { get; set; } = default!;

    /// <summary>
    /// Path to an xml comments file. This options. Commentario will still produce documentation purely from a source assembly.
    /// </summary>
    public string XmlCommentsPath { get; set; } = default!;

    /// <summary>
    /// Path to a readme file. This is optional. If specified, the contents will be included at the start of the documentation.
    /// Contents must be in html format.
    /// </summary>
    public string ReadMePath { get; set; } = default!;

    /// <summary>
    /// Optional path to a styles file. If specified, the contents must be in the format suitable for the output type.
    /// For example, for html output, the styles file must be a valid .css file.
    /// </summary>
    public string StylesPath { get; set; } = default!;

    /// <summary>
    /// The output type. Currently only Html output is supported.
    /// </summary>
    public OutputType OutputType { get; set; } = OutputType.html;

    /// <summary>
    /// If set to true, the the output file will be overwritten if it exists. Otherwise, an error will be thrown if the output file exists.
    /// </summary>
    public bool AllowOverwrite { get; set; } = false;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}