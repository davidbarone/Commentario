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

    /// <summary>
    /// Static factory method to create a new DocumentGenerator instance. 
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly file.</param>
    /// <param name="outputPath">The path to the output documentation file.</param>
    /// <param name="outputType">The documentation format type.</param>
    /// <param name="xmlCommentsPath">The path to an xml comments file. This content will be merged with the reflected assembly information.</param>
    /// <param name="readMePath">The path to an optional readme file. This file should contain html content. If a file is specified, it will be included at the top of the documentation file.</param>
    /// <returns>Returns a DocumentGenerator instance.</returns>
    /// <exception cref="Exception">Throws an exception if invalid parameters are provided.</exception>
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

    private bool IsCompilerGenerated(Type type)
    {
        var attr = type.CustomAttributes.Select(t => t.AttributeType).Where(t => t.Name == "CompilerGeneratedAttribute").ToArray();
        return attr.Any();
    }

    protected Type[] GetTypes()
    {
        var assembly = GetAssembly();
        var types = assembly.GetTypes().OrderBy(t => t.Name).ToArray();

        // remove compiler-generated / lambda classes
        types = types.Where(t => !IsCompilerGenerated(t)).ToArray();

        return types;
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
            <h2>Table of Contents</h2>
            <div class=""toc-inner"">
                {this.RenderTOCSection("Classes", this.GetClasses())}
                {this.RenderTOCSection("Structs", this.GetStructs())}
                {this.RenderTOCSection("Interfaces", this.GetInterfaces())}
                {this.RenderTOCSection("Enums", this.GetEnums())}
            </div>
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

    /* https://goodpalette.io */
    :root {

        /* Primary: Blue Blue */
        --primary-100: #F3F2FF;
        --primary-200: #C0C0FE;
        --primary-300: #8B91F9;
        --primary-400: #5565E9;
        --primary-500: #233FCC;
        --primary-600: #0C2EA3;
        --primary-700: #032479;
        --primary-800: #001B50;
        --primary-900: #000F26;

        /* Accent: Jaffa */
        --accent-100: #FFF9F2;
        --accent-200: #FFE4C8;
        --accent-300: #FBC89D;
        --accent-400: #F3A470;
        --accent-500: #E37944;
        --accent-600: #B44921;
        --accent-700: #852A11;
        --accent-800: #551509;
        --accent-900: #260704;

        /* Neutral */
        --neutral-100: #FAFAFC;
        --neutral-200: #EAEAEF;
        --neutral-300: #DADAE2;
        --neutral-400: #CACBD4;
        --neutral-500: #BBBDC7;
        --neutral-600: #94969F;
        --neutral-700: #6D7077;
        --neutral-800: #474A4E;
        --neutral-900: #222426;
    }

    /* -----------------------------------------------
    Base Styles
    -------------------------------------------------- */

    body {
        font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;
        color: var(--neutral-900);
        overflow-y: scroll;
        font-size: 0.8em;
    }

    div.toc {
        background-color: var(--primary-200);
        border: 1px solid var(--primary-500);
        border-radius: 4px;
        margin: 4px 0px;
        padding: 0px;
    }

    div.toc-inner {
        margin: 4px;
    }

    div.type {
        background-color: var(--accent-200);
        border: 1px solid var(--accent-500);
        border-radius: 4px;
        margin: 4px 0px;
        padding: 0px;
    }

    div.type-inner {
        margin: 4px;
    }

    div.member {
        background-color: var(--primary-200);
        border: 1px solid var(--primary-500);
        border-radius: 4px;
        margin: 4px 0px;
        padding: 4px 4px;
    }

    div.member-inner {
        margin: 4px;
    }

    /* ------------------------------------
    Typography
    --------------------------------------- */

    h1, h2, h3, h4, h5, h6 {
        font-weight: 500;
    }

    h1 {
        font-size: 2.0em;
        display: block;
        background: var(--primary-300);
        border: 1px solid var(--primary-700);
        border-left: 2em solid var(--primary-700);
        border-right: 2em solid var(--primary-700);
        padding: 24px 6px;
        border-radius: 4px;
    }

    h2 {
        font-size: 1.8em;
    }

    h3 {
        font-size: 1.6em;
    }

    div.toc h2  {
        border-left: 2.0em solid var(--primary-500);
        display: block;
        padding: 12px 2px 12px 12px;
        background-color: var(--primary-300);
        margin: 0px;
    }

    div.toc h3 {
        color: var(--primary-500);
    }

    div.type h2  {
        border-left: 2.0em solid var(--accent-500);
        display: block;
        padding: 12px 2px 12px 12px;
        background-color: var(--accent-300);
        margin: 0px;
    }

    div.type h3 {
        color: var(--accent-500);
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
        margin: 0px;
        padding: 0px;
    }

    ol {
        list-style: decimal inside;
    }

    ol, ul {
        padding-left: 0;
    }

    ul ul,
    ul ol,
    ol ol,
    ol ul {
    }

    li {
    }

    /* ---------------------------------------------
    Code / Pre
    ------------------------------------------------ */

    pre code {
        background-color: var(--neutral-200);
        border: 1px solid var(--neutral-700);
        border-left: 6px solid var(--accent-700);
        color: var(--neutral-900);
        page-break-inside: avoid;
        font: ""Courier New"", Courier, Monospace;
        max-width: 100%;
        padding: 2em 1em;
        display: inline-block;
        word-wrap: break-word;
        overflow: auto;
        overflow-x: auto;
        white-space: pre-wrap;
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

    div.table {
        border: 1px solid var(--primary-700);
        display: inline-block;
        border-radius: 4px;
        padding: 0px;
        margin: 0px;
        overflow: hidden
    }

    table {
        border-collapse: separate;
        border-spacing: 0;
        padding: 0px;
    }

    th,
    td {
        padding: 6px 24px;
        text-align: left;
        margin: 0px;
    }

    th {
        background-color: var(--primary-700);
        color: var(--neutral-300);
        font-weight: 500;
    }

    tr:nth-child(odd) {
        background-color: var(--neutral-100);  
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
