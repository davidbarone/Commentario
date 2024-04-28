using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Main entry point to CommentarioServer. Generates documents from xml comments and the assembly file.
/// </summary>
public class DocumentGenerator
{
    /// <summary>
    /// Creates a new DocumentGenerator instance.
    /// </summary>
    public DocumentGenerator(string xmlCommentsPath, string assemblyPath, string outputPath, OutputType outputType)
    {
        OutputType = outputType;
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

    private IEnumerable<Type> GetTypes()
    {
        string inspectedAssembly = this.AssemblyPath;
        var resolver = new PathAssemblyResolver(new string[] { inspectedAssembly, typeof(object).Assembly.Location });
        using var mlc = new MetadataLoadContext(resolver, typeof(object).Assembly.GetName().ToString());

        // Load assembly into MetadataLoadContext
        Assembly assembly = mlc.LoadFromAssemblyPath(inspectedAssembly);
        AssemblyName name = assembly.GetName();

        return assembly.GetTypes().OrderBy(t => t.Name);
    }

    private ConstructorInfo[] GetConstructors(Type type)
    {
        return type.GetConstructors();
    }

    private MethodInfo[] GetMethods(Type type) {
        return type.GetMethods();
    }

    private PropertyInfo[] GetProperties(Type type) {
        return type.GetProperties();
    }

    /// <summary>
    /// Generates document.
    /// </summary>
    public void GenerateDocument()
    {
        Validation();


    }



}
