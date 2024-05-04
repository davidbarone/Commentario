using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Main entry point to CommentarioServer. Generates documents from xml comments and the assembly file.
/// </summary>
public abstract class DocumentGenerator
{
    #region Properties

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

    /// <summary>
    /// Optional path to an assembly readme. The contents are included at the top of the documentation file. 
    /// </summary>
    public string ReadMePath { get; set; } = default!;

    #endregion

    #region Constructors

    public static DocumentGenerator Create(string xmlCommentsPath, string assemblyPath, string readMePath, string outputPath, OutputType outputType)
    {
        switch (outputType)
        {
            case OutputType.Html:
                return new HtmlDocumentGenerator(xmlCommentsPath, assemblyPath, readMePath, outputPath);
            default:
                throw new Exception($"Output type {outputType.ToString()} not supported.");
        }
    }

    /// <summary>
    /// Creates a new DocumentGenerator instance.
    /// </summary>
    internal DocumentGenerator(string xmlCommentsPath, string assemblyPath, string readMePath, string outputPath)
    {
        ReadMePath = readMePath;
        XmlCommentsPath = xmlCommentsPath;
        AssemblyPath = assemblyPath;
        OutputPath = outputPath;
    }

    #endregion

    #region Methods

    private void Validation()
    {
        if (string.IsNullOrEmpty(this.AssemblyPath))
        {
            throw new Exception("AssemblyPath not set!");
        }

        if (!string.IsNullOrEmpty(this.ReadMePath) && !File.Exists(this.ReadMePath))
        {
            throw new Exception($"ReadMe file [{this.ReadMePath}] not found!");
        }
    }

    #endregion

    protected Type[] GetTypes()
    {
        // Use MetadataLoadContext to inspect types
        // Need to provide all BCL libraries in search too.
        string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
        var paths = new List<string>(runtimeAssemblies);
        paths.Add(this.AssemblyPath);
        var resolver = new PathAssemblyResolver(paths);

        var mlc = new MetadataLoadContext(resolver, typeof(object).Assembly.GetName().ToString());

        // Load assembly into MetadataLoadContext
        Assembly assembly = mlc.LoadFromAssemblyPath(this.AssemblyPath);

        return assembly.GetTypes().OrderBy(t => t.Name).ToArray();
    }

    protected string GetTypeCategory(Type type)
    {
        if (type.IsClass)
        {
            return "Class";
        }
        else if (type.IsValueType)
        {
            return "Struct";
        }
        else if (type.IsInterface)
        {
            return "Interface";
        }
        else if (type.IsEnum)
        {
            return "Enum";
        }
        else
        {
            return "Other";
        }
    }

    protected Type[] GetClasses()
    {
        var types = GetTypes();
        return types.Where(t => t.IsClass).ToArray();
    }

    protected Type[] GetStructs()
    {
        return GetTypes().Where(t => t.IsValueType).ToArray();
    }

    protected Type[] GetInterfaces()
    {
        return GetTypes().Where(t => t.IsInterface).ToArray();
    }

    protected Type[] GetEnums()
    {
        return GetTypes().Where(t => t.IsEnum).ToArray();
    }

    protected ConstructorInfo[] GetConstructors(Type type)
    {
        return type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
    }

    protected MethodInfo[] GetMethods(Type type)
    {
        // ignore getter/setter methods.
        return type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => !m.IsSpecialName).ToArray();
    }

    protected PropertyInfo[] GetProperties(Type type)
    {
        return type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
    }

    protected EventInfo[] GetEvents(Type type)
    {
        return type.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
    }

    protected string GetReadMe()
    {
        if (!string.IsNullOrEmpty(this.ReadMePath) && File.Exists(this.ReadMePath))
        {
            return File.ReadAllText(this.ReadMePath);
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Generates document.
    /// </summary>
    public void GenerateDocument()
    {
        Validation();
        var contentTypes = "";

        // Get documentation
        var comments = new XmlCommentsReader(this.XmlCommentsPath).Document;

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
  </head>
  <body id=""top"">
        <h1>{comments.Assembly.Name}</h1>
        {this.GetReadMe()}

        {this.RenderTOCSection("Classes", this.GetClasses(), comments)}
        {this.RenderTOCSection("Structs", this.GetStructs(), comments)}
        {this.RenderTOCSection("Interfaces", this.GetInterfaces(), comments)}
        {this.RenderTOCSection("Enums", this.GetEnums(), comments)}

        <hr />

        {contentTypes}
    </body>
</html>
        ";

        // Save file
        File.WriteAllText(this.OutputPath, template);
    }

    protected abstract string RenderTOCSection(string header, Type[] types, DocumentNode comments);
    protected abstract string RenderType(Type type);
    protected abstract string RenderTypeTOCSection(Type type, MemberInfo[] members);
    protected abstract string RenderTypeMember(MemberInfo member);

    protected string GetCSSStyles()
    {
        var css = @"
<style type=""text/css"">

    /* -----------------------------------------------
    Base Styles
    -------------------------------------------------- */

    body {
        font-family: sans-serif, ""Helvetica Neue"", Helvetica, Arial;
        color: #222;
        overflow-y: scroll;
    }

    /* ------------------------------------
    Typography
    --------------------------------------- */

    h1, h2, h3, h4, h5, h6 {
        font-weight: 300;
        margin-top: 0.5em;
        margin-bottom: 0.5em;
    }

    h1 {
        font-size: 2.0em;
        letter-spacing: -.05em;
    }

    h2 {
        font-size: 1.8em;
        letter-spacing: -.05em;
    }

    h3 {
        font-size: 1.6em;
        letter-spacing: -.05em;
    }

    h4 {
        font-size: 1.4em;
        letter-spacing: -.025em;
    }

    h5 {
        font-size: 1.2em;
        letter-spacing: -.025em;
    }

    h6 {
        font-size: 1.0em;
        letter-spacing: 0;
    }

    /* -----------------------------------
    Links
    -------------------------------------- */

    a {
        color: #456789;
        text-decoration: none;
    }

    a:hover {
        color: #123456;
        text-decoration: underline;
    }

    /* ---------------------------------------------
    definitions
    ------------------------------------------------ */

    dl {
        border: 3px double #ccc;
        padding: 0.5em;
    }

    dl * {
        display: none;
        float: left;
    }

    dl:after {
        content: "";
        display: table;
        clear: both;
    }

    dt {
        visibility: visible;
        width: 25%;
        text-align: right;
        font-weight: bold;
        display: inline-block;
        color: rgb(61, 79, 93);
        box-sizing: border-box;
        padding-right: 3px;
        margin: 0px;
    }

    dt:after {
        content: ':';
    }

    dd {
        visibility:visible;
        width: 75%;
        text-align: left;
        display: inline-block;
        box-sizing: border-box;
        padding-left: 3px;
        margin: 0px;
    }

    /* ---------------------------------------------
    Lists
    ------------------------------------------------ */

    ul {
        list-style: disc inside;
    }

    ol {
        list-style: decimal inside;
    }

    ol, ul {
        padding-left: 0;
        margin-top: 0.5em;
    }

    ul ul,
    ul ol,
    ol ol,
    ol ul {
        margin: 0.5em 0 0.5em 3em;
        font-size: 90%;
    }

    li {
        margin-bottom: 0.25em;
    }

    /* ---------------------------------------------
    Code / Pre
    ------------------------------------------------ */

    code {
        padding: .2em .5em;
        margin: 0 .2em;
        font-size: 90%;
        white-space: nowrap;
        background: #F1F1F1;
        border: 1px solid #E1E1E1;
        border-radius: 4px;
    }

    pre {
        font-family: sans-serif, ""Helvetica Neue"", Helvetica, Arial;
    }

    pre > code {
        display: block;
        padding: 1em 1.5em;
        white-space: pre;
    }

    /* ---------------------------------------------
    Blockquote + cite
    ------------------------------------------------ */

    blockquote {
        border-left: 10px solid rgb(61, 79, 93);
        background: #f9f9f9;
        font-family: Georgia, serif;
        font-style: italic;
        margin: 0.25em 0;
        padding: 0.25em 60px;
        line-height: 1.45;
        position: relative;
        color: #383838;
    }

    blockquote:before {
        display: block;
        content: ""\201C"";
        font-size: 60px;
        position: absolute;
        left: 20px;
        top: 0px;
        color: #7a7a7a;
    }

    blockquote cite {
        color: #999999;
        font-size: 14px;
        display: block;
        margin-top: 5px;
    }
    
    blockquote cite:before {
        content: ""\2014 \2009"";
    }

    /* ---------------------------------------------
    Tables
    ------------------------------------------------ */

    th,
    td {
        padding: 6px 24px;
        text-align: left;
        margin: 0px;
    }

    th {
        background-color: #123;
        color: white;
        font-weight: 500;
    }

    tr:nth-child(odd) {
        background-color: #eee;  
    }

    /* ---------------------------------------------
    Spacing
    ------------------------------------------------ */

    pre,
    blockquote,
    dl,
    figure,
    table,
    p,
    ul,
    ol,
    {
        margin-bottom: .5em;
    }

    /* ---------------------------------------------
    Misc
    ------------------------------------------------ */

    hr {
        margin-top: 2em;
        margin-bottom: 2em;
        border-width: 0;
        border-top: 4px solid #123;
    }

</style>
        ";

        return css;
    }
}
