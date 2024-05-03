using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Main entry point to CommentarioServer. Generates documents from xml comments and the assembly file.
/// </summary>
public abstract class DocumentGenerator
{
    public static DocumentGenerator Create(string xmlCommentsPath, string assemblyPath, string outputPath, OutputType outputType)
    {
        switch (outputType)
        {
            case OutputType.Html:
                return new HtmlDocumentGenerator(xmlCommentsPath, assemblyPath, outputPath);
            default:
                throw new Exception($"Output type {outputType.ToString()} not supported.");
        }
    }

    /// <summary>
    /// Creates a new DocumentGenerator instance.
    /// </summary>
    internal DocumentGenerator(string xmlCommentsPath, string assemblyPath, string outputPath)
    {
        XmlCommentsPath = xmlCommentsPath;
        AssemblyPath = assemblyPath;
        OutputPath = outputPath;
    }

    /// <summary>
    /// The document output format.
    /// </summary>
    public OutputType OutputType { get; set; }

    /// <summary>
    /// The xml comments path.
    /// </summary>
    public string XmlCommentsPath { get; set; } = default!;

    /// <summary>
    /// The assembly file path.
    /// </summary>
    public string AssemblyPath { get; set; } = default!;

    /// <summary>
    /// The documentation output path. 
    /// </summary>
    public string OutputPath { get; set; } = default!;

    public void Validation()
    {
        if (string.IsNullOrEmpty(this.AssemblyPath))
        {
            throw new Exception("AssemblyPath not set!");
        }
    }

    private Type[] GetTypes()
    {
        string inspectedAssembly = this.AssemblyPath;
        var resolver = new PathAssemblyResolver(new string[] { inspectedAssembly, typeof(object).Assembly.Location });
        var mlc = new MetadataLoadContext(resolver, typeof(object).Assembly.GetName().ToString());

        // Load assembly into MetadataLoadContext
        Assembly assembly = mlc.LoadFromAssemblyPath(inspectedAssembly);
        AssemblyName name = assembly.GetName();

        return assembly.GetTypes().OrderBy(t => t.Name).ToArray();
    }

    private ConstructorInfo[] GetConstructors(Type type)
    {
        return type.GetConstructors();
    }

    private MethodInfo[] GetMethods(Type type)
    {
        return type.GetMethods();
    }

    private PropertyInfo[] GetProperties(Type type)
    {
        return type.GetProperties();
    }

    /// <summary>
    /// Generates document.
    /// </summary>
    public void GenerateDocument()
    {
        Validation();
        var contentCSSReset = GetCSSReset();
        var contentTOC = RenderTOC(this.GetTypes());
        var contentTypes = "";
        foreach (var type in this.GetTypes())
        {
            contentTypes += RenderType(type);
        }

        var template = @$"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""ie=edge"">
    <title>HTML 5 Boilerplate</title>
    {this.GetCSSStyles()}
    <style type=""text/css"">
        {this.GetCSSReset()}
    </style>
  </head>
  <body>
          {contentTOC}
            {contentTypes}
    </body>
</html>
        ";

        // Save file
        File.WriteAllText(this.OutputPath, template);
    }

    protected abstract string RenderTOC(Type[] types);

    protected abstract string RenderType(Type type);

    protected string GetCSSReset()
    {
        return @"/*! normalize.css v8.0.1 | MIT License | github.com/necolas/normalize.css */html{line-height:1.15;-webkit-text-size-adjust:100%}body{margin:0}main{display:block}h1{font-size:2em;margin:.67em 0}hr{box-sizing:content-box;height:0;overflow:visible}pre{font-family:monospace,monospace;font-size:1em}a{background-color:transparent}abbr[title]{border-bottom:none;text-decoration:underline;text-decoration:underline dotted}b,strong{font-weight:bolder}code,kbd,samp{font-family:monospace,monospace;font-size:1em}small{font-size:80%}sub,sup{font-size:75%;line-height:0;position:relative;vertical-align:baseline}sub{bottom:-.25em}sup{top:-.5em}img{border-style:none}button,input,optgroup,select,textarea{font-family:inherit;font-size:100%;line-height:1.15;margin:0}button,input{overflow:visible}button,select{text-transform:none}[type=button],[type=reset],[type=submit],button{-webkit-appearance:button}[type=button]::-moz-focus-inner,[type=reset]::-moz-focus-inner,[type=submit]::-moz-focus-inner,button::-moz-focus-inner{border-style:none;padding:0}[type=button]:-moz-focusring,[type=reset]:-moz-focusring,[type=submit]:-moz-focusring,button:-moz-focusring{outline:1px dotted ButtonText}fieldset{padding:.35em .75em .625em}legend{box-sizing:border-box;color:inherit;display:table;max-width:100%;padding:0;white-space:normal}progress{vertical-align:baseline}textarea{overflow:auto}[type=checkbox],[type=radio]{box-sizing:border-box;padding:0}[type=number]::-webkit-inner-spin-button,[type=number]::-webkit-outer-spin-button{height:auto}[type=search]{-webkit-appearance:textfield;outline-offset:-2px}[type=search]::-webkit-search-decoration{-webkit-appearance:none}::-webkit-file-upload-button{-webkit-appearance:button;font:inherit}details{display:block}summary{display:list-item}template{display:none}[hidden]{display:none}
/*# sourceMappingURL=normalize.min.css.map */";
    }

    protected string GetCSSStyles()
    {
        // Uses Pico CSS framework
        // https://picocss.com/
        return @"<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/@picocss/pico@2/css/pico.min.css""/>";
    }
}
