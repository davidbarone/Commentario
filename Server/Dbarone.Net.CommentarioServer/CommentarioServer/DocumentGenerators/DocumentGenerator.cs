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
    /// The assembly file path.
    /// </summary>
    public string AssemblyPath { get; set; } = default!;

    /// <summary>
    /// The documentation output path. 
    /// </summary>
    public string OutputPath { get; set; } = default!;

    /// <summary>
    /// The document output format.
    /// </summary>
    public OutputType OutputType { get; set; }

    /// <summary>
    /// Set to true to allow the output documentation file to be overwritten. Otherwise an exception will be throw if the file exists.
    /// </summary>
    public bool AllowOverwrite { get; set; } = false;

    /// <summary>
    /// The xml comments path.
    /// </summary>
    public string? XmlCommentsPath { get; set; } = default!;

    /// <summary>
    /// Optional path to an assembly readme. The contents are included at the top of the documentation file. 
    /// </summary>
    public string? ReadMePath { get; set; } = default!;

    /// <summary>
    /// Optional path to a styles file. For html output, the file must be a valid css file.
    /// </summary>
    public string? StylesPath { get; set; } = default!;

    /// <summary>
    /// The styles content.
    /// </summary>
    protected string Styles { init; get; } = default!;

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
    /// <param name="allowOverwrite">Set to true to allow existing output documentation file to be overwritten. Otherwise an exception will be thrown if the output file exists.
    /// <param name="xmlCommentsPath">The path to an xml comments file. This content will be merged with the reflected assembly information.</param>
    /// <param name="readMePath">The path to an optional readme file. This file should contain html content. If a file is specified, it will be included at the top of the documentation file.</param>
    /// <param name="stylesPath">The path to an optional styles file.</param>
    /// <returns>Returns a <see cref="DocumentGenerator"/> instance.</returns>
    /// <exception cref="Exception">Throws an exception if invalid parameters are provided.</exception>
    public static DocumentGenerator Create(string assemblyPath, string outputPath, OutputType outputType = OutputType.html, bool allowOverwrite = false, string? xmlCommentsPath = null, string? readMePath = null, string? stylesPath = null)
    {
        switch (outputType)
        {
            case OutputType.html:
                return new HtmlDocumentGenerator(
                    assemblyPath,
                    outputPath,
                    allowOverwrite,
                    xmlCommentsPath,
                    readMePath,
                    stylesPath);
            default:
                throw new Exception($"Output type {outputType.ToString()} not supported.");
        }
    }

    /// <summary>
    /// Creates a new DocumentGenerator instance.
    /// </summary>
    internal DocumentGenerator(
        string assemblyPath,
        string outputPath,
        bool allowOverwrite,
        string? xmlCommentsPath,
        string? readMePath,
        string? stylesPath)
    {
        AssemblyPath = assemblyPath;
        OutputPath = outputPath;
        AllowOverwrite = allowOverwrite;
        XmlCommentsPath = xmlCommentsPath;
        ReadMePath = readMePath;
        StylesPath = stylesPath;

        // styles
        if (this.StylesPath != null && File.Exists(this.StylesPath))
        {
            this.Styles = File.ReadAllText(this.StylesPath);
        }
        else
        {
            this.Styles = this.DefaultStyles;
        }

        // Get xml comments document.
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

        if (!string.IsNullOrEmpty(this.StylesPath) && !File.Exists(this.StylesPath))
        {
            throw new Exception($"Styles file [{this.ReadMePath}] not found!");
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

        // Include files in assembly directory
        string[] sourceAssemblies = Directory.GetFiles(new FileInfo(this.AssemblyPath).DirectoryName!, "*.dll");

        var paths = new List<string>(runtimeAssemblies);
        paths.AddRange(sourceAssemblies);
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
        return type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        .Where(c => !IsPrivate(c)).ToArray();
    }

    protected MethodInfo[] GetMethods(Type type)
    {
        // ignore getter/setter methods.
        return type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName)
            .Where(m => !IsPrivate(m)).ToArray()
            .ToArray();
    }

    protected FieldInfo[] GetFields(Type type)
    {
        return type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(f => !IsPrivate(f)).ToArray();
    }

    protected PropertyInfo[] GetProperties(Type type)
    {
        return type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(p => !IsPrivate(p)).ToArray();

    }

    protected EventInfo[] GetEvents(Type type)
    {
        return type.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(e => !IsPrivate(e)).ToArray();
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
        if (this.AllowOverwrite == false && File.Exists(this.OutputPath))
        {
            throw new Exception("Output file already exists and [Allow Overwrite] parameter set to false.");
        }
        File.WriteAllText(this.OutputPath, RenderDocument());
    }

    protected abstract string RenderDocument();
    protected abstract string RenderTOCSection(string header, Type[] types);
    protected abstract string RenderType(Type type);
    protected abstract string RenderTypeTOCSection(Type type, string header, MemberInfo[] members);
    protected abstract string RenderTypeFields(Type type);
    protected abstract string RenderTypeProperties(Type type);
    protected abstract string RenderTypeMethods(Type type);
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
    protected abstract string DefaultStyles { get; }

    /// <summary>
    /// Renders the type's generic arguments.
    /// </summary>
    /// <param name="type">The type to render.</param>
    /// <returns>A string representing the generic arguments.</returns>
    protected abstract string RenderTypeGenericArguments(Type type);

    /// <summary>
    /// Returns a hyperlink for a type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected string GetLinkForType(Type type)
    {
        var types = this.GetTypes();
        if (types.Select(t => t.ToCommentId()).Contains(type.ToCommentId()))
        {
            return @$"<a href=""#{type.ToCommentId()}"">{type.Name}</a>";
        }
        else
        {
            return type.Name;
        }
    }
}
