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

    /// <summary>
    /// The xml comments.
    /// </summary>
    public DocumentNode Comments { get; set; }

    #endregion

    #region Constructors

    public static DocumentGenerator Create(string assemblyPath, string outputPath, OutputType? outputType = OutputType.html, string? xmlCommentsPath = null, string? readMePath = null)
    {
        switch (outputType)
        {
            case OutputType.html:
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

        // Get documentation
        Comments = new XmlCommentsReader(this.XmlCommentsPath).Document;
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

    private Assembly GetAssembly()
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
        return assembly;

    }

    protected string GetAssemblyName()
    {
        return GetAssembly().GetName().Name;
    }

    protected Type[] GetTypes()
    {
        var assembly = GetAssembly();
        return assembly.GetTypes().OrderBy(t => t.Name).ToArray();
    }

    public Type? GetInheritedType(Type type)
    {
        return type.BaseType;
    }

    public Type[] GetInterfacesImplemented(Type type)
    {
        return type.GetInterfaces();
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

    protected MemberInfo[] GetMembers(Type type)
    {
        return type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
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

    protected FieldInfo[] GetFields(Type type)
    {
        return type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
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

    protected ParameterInfo[] GetMemberParameters(MemberInfo member)
    {
        var method = member as MethodInfo;
        if (method is not null)
        {
            return method.GetParameters();
        }
        else
        {
            return new ParameterInfo[0];
        }
    }

    /// <summary>
    /// Generates document.
    /// </summary>
    public void GenerateDocument()
    {
        Validation();
        var contentTypes = "";
        var contentMembers = "";

        foreach (var type in this.GetTypes())
        {
            contentTypes += RenderType(type);
            foreach (var member in GetMembers(type))
            {
                contentMembers += RenderTypeMember(member);
            }
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
        <h1>{GetAssembly()}</h1>
        {this.GetReadMe()}

        <div class=""toc"">
            {this.RenderTOCSection("Classes", this.GetClasses())}
            {this.RenderTOCSection("Structs", this.GetStructs())}
            {this.RenderTOCSection("Interfaces", this.GetInterfaces())}
            {this.RenderTOCSection("Enums", this.GetEnums())}
        </div>

        {contentTypes}
        {contentMembers}

    </body>
</html>
        ";

        // Save file
        File.WriteAllText(this.OutputPath, template);
    }

    protected abstract string RenderTOCSection(string header, Type[] types);
    protected abstract string RenderType(Type type);
    protected abstract string RenderTypeTOCSection(Type type, string header, MemberInfo[] members);
    protected abstract string RenderTypeMember(MemberInfo member);

    protected abstract string RenderItems(object[] items);

    protected abstract string RenderExample(ExampleNode node);

    protected abstract string RenderCode(CodeNode node);

    protected abstract string RenderC(CNode node);

    protected abstract string RenderPara(ParaNode node);

    protected abstract string RenderSee(SeeNode node);

    protected abstract string RenderReturns(ReturnsNode node);

    protected abstract string RenderExceptions(ExceptionNode[] nodes);
    protected abstract string RenderException(ExceptionNode node);
    protected abstract string RenderParam(ParamNode? node);

    /// <summary>
    /// Renders the type's generic arguments.
    /// </summary>
    /// <param name="type">The type to render.</param>
    /// <returns>A string representing the generic arguments.</returns>

    protected abstract string RenderTypeGenericArguments(Type type);

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
        font-size: 0.8em;
        background-color: #fbefcc
    }

    div.type {
        background-color: white;
        border: 1px solid black;
        border-radius: 4px;
        margin: 4px 0px;
        padding: 4px 4px;
    }

    div.member {
        background-color: #daebe8;
        border: 1px solid black;
        border-radius: 4px;
        margin: 4px 0px;
        padding: 4px 4px;
    }

    div.toc {
        background-color: #f9ccac;
        border: 1px solid black;
        border-radius: 4px;
        margin: 4px 0px;
        padding: 4px 4px;
    }

    /* ------------------------------------
    Typography
    --------------------------------------- */

    h1, h2, h3, h4, h5, h6 {
        font-weight: 300;
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

    pre > code {
        display: inline-block;
        box-sizing: border-box;
        padding: 1em 2em;
        margin: 1em 0em;
        white-space: pre;
        font-size: 100%;
        border: 1px solid #123;
        border-radius: 4px;
        background-color: #e0e8ef;
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
