using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Main entry point to CommentarioServer. Generates documents from xml comments and the assembly file.
/// </summary>
/// <remarks>
/// Note that this class is an abstract base class. Implementations are included in sub classes.
/// </remarks>
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

    private void ValidateParameters()
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

    /// <summary>
    /// Gets the assembly referenced by the <c>AssemblyPath</c> parameter.
    /// </summary>
    /// <remarks>
    /// Uses MetadataLoadContext to load assembly metadata only for inspection.
    /// </remarks>
    /// <returns>The Assembly object.</returns>
    protected Assembly GetAssembly()
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

    private bool IsCompilerGenerated(Type type)
    {
        var attr = type.CustomAttributes.Select(t => t.AttributeType).Where(t => t.Name == "CompilerGeneratedAttribute").ToArray();
        return attr.Any();
    }

    /// <summary>
    /// Gets all types in the source assembly.
    /// </summary>
    /// <returns>Returns an array of <c>Type</c> objects.</returns>
    protected Type[] GetTypes()
    {
        var assembly = GetAssembly();
        var types = assembly.GetTypes().OrderBy(t => t.Name).ToArray();

        // remove compiler-generated / lambda classes
        types = types.Where(t => !IsCompilerGenerated(t)).ToArray();

        return types;
    }

    /// <summary>
    /// Gets the inherited or base type, if any.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>Returns the base type, or null.</returns>
    protected Type? GetInheritedType(Type type)
    {
        return type.BaseType;
    }

    /// <summary>
    /// Gets the subclasses - types which directly inherit from the specified type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>Returns an array of types that directly inherit from the specified type.</returns>
    protected Type[] GetSubClasses(Type type)
    {
        return this.GetTypes()
            .Where(t => t.BaseType != null && (t.BaseType.Name == type.Name)).ToArray();
    }

    /// <summary>
    /// Gets the interfaces implemented by a type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>Returns an array of interface types implemented by the specified type.</returns>
    protected Type[] GetInterfacesImplemented(Type type)
    {
        return type.GetInterfaces();
    }

    /// <summary>
    /// Gets the category of a type.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Category</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>Class</term>
    /// <description>The type is a class (reference type).</description>
    /// </item>
    /// <item>
    /// <term>Struct</term>
    /// <description>The type is a struct (value type).</description>
    /// </item>
    /// <item>
    /// <term>Interface</term>
    /// <description>The type is an interface.</description>
    /// </item>
    /// <item>
    /// <term>Enum</term>
    /// <description>The type is an enum.</description>
    /// </item>
    /// <item>
    /// <term>Other</term>
    /// <description>The type is unspecified.</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="type">The type to check.</param>
    /// <returns>Returns a string value to categorise the type:</returns>
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

    /// <summary>
    /// Gets a list of classes for the source Assembly.
    /// </summary>
    /// <returns>Returns an array of class types.</returns>
    protected Type[] GetClasses()
    {
        var types = GetTypes();
        return types.Where(t => t.IsClass).ToArray();
    }

    /// <summary>
    /// Gets a list of structs for the source Assembly.
    /// </summary>
    /// <returns>Returns an array of struct types.</returns>
    protected Type[] GetStructs()
    {
        return GetTypes().Where(t => t.IsValueType).ToArray();
    }

    /// <summary>
    /// Gets a list of interfaces for the source Assembly.
    /// </summary>
    /// <returns>Returns an array of interface types.</returns>
    protected Type[] GetInterfaces()
    {
        return GetTypes().Where(t => t.IsInterface).ToArray();
    }

    /// <summary>
    /// Gets a list of enums for the source Assembly.
    /// </summary>
    /// <returns>Returns an array of enum types.</returns>
    protected Type[] GetEnums()
    {
        return GetTypes().Where(t => t.IsEnum).ToArray();
    }

    private bool IsPrivate(MemberInfo member)
    {
        // returns true for public + protected members.
        if (member.MemberType == MemberTypes.Method)
        {
            return (member as MethodBase).IsPrivate;
        }
        else if (member.MemberType == MemberTypes.Property)
        {
            return (member as PropertyInfo).GetMethod.IsPrivate;
        }
        else if (member.MemberType == MemberTypes.Constructor)
        {
            return (member as ConstructorInfo).IsPrivate;
        }
        else if (member.MemberType == MemberTypes.Field)
        {
            return (member as FieldInfo).IsPrivate;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Gets public or protected members for a type.
    /// </summary>
    /// <param name="type">The type to get members for.</param>
    /// <returns>Returns all public and protected members.</returns>
    protected MemberInfo[] GetMembers(Type type)
    {
        return type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !IsPrivate(m)).ToArray();
    }

    protected ConstructorInfo[] GetConstructors(Type type)
    {
        return type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    protected MethodInfo[] GetMethods(Type type)
    {
        // ignore getter/setter methods.
        return type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Where(m => !m.IsSpecialName).ToArray();
    }

    protected FieldInfo[] GetFields(Type type)
    {
        return type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    protected PropertyInfo[] GetProperties(Type type)
    {
        return type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    }

    protected EventInfo[] GetEvents(Type type)
    {
        return type.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
        ValidateParameters();

        // Save file
        File.WriteAllText(this.OutputPath, RenderDocument());
    }

    protected abstract string RenderDocument();
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

    protected abstract string RenderList(ListNode node);

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

        --white: #fff;
    }

    /* -----------------------------------------------
    Base Styles
    -------------------------------------------------- */

    body {
        font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif;
        color: var(--neutral-900);
        overflow-y: scroll;
        font-size: 0.9em;
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
        background-color: var(--neutral-200);
        border: 1px solid var(--neutral-500);
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
        margin: 1em 0em 0em 0em;
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

    div.member h2  {
        border-left: 2.0em solid var(--primary-500);
        display: block;
        padding: 12px 2px 12px 12px;
        background-color: var(--primary-300);
        margin: 0px;
    }

    div.member h3 {
        color: var(--primary-500);
    }

    /* -----------------------------------
    Links
    -------------------------------------- */

    a {
        color: var(--accent-700);
        text-decoration: none;
    }

    a:hover {
        color: var(--accent-700);
        text-decoration: underline;
    }

    /* ---------------------------------------------
    Lists
    ------------------------------------------------ */

    ol, ul {
        padding: 0px;
        margin: 0px;
        margin-bottom: 0.5em;
    }

    ul {
        list-style: disc inside;
    }

    ol {
        list-style: decimal inside;
    }

    /* ---------------------------------------------
    Code / Pre
    ------------------------------------------------ */

    pre code {
        background-color: var(--white);
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
        font-weight: 500;
    }

    code {
        font-weight: bold;
        background-color: var(--neutral-500);
        padding: 2px;
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
        margin-top: 0em;
        margin-bottom: .5em;
    }

</style>
        ";

        return css;
    }
}
