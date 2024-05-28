using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Describes a single member in the xml comments. A member can be either a .NET Type or a .NET member for a type (for example a method, property, field etc).
/// </summary>
public class MemberNode
{
    /// <summary>
    /// The name of the member.
    /// </summary>
    /// <remarks>
    /// The format of the name is called an ID String: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/
    /// The string uniquely identifies the member.
    /// </remarks>
    /// <example>
    /// Using the code example shown below:
    /// <code>
    /// public class Test
    /// {
    ///     public int Add(int a, int b)
    ///     {
    ///         return a + b;
    ///     }
    /// }
    /// </code>
    /// The above method could generate a name of <c>M:ExampleLibrary.Test.Add(System.Int32,System.Int32)</c>
    /// <para>
    /// Note that the return type is not included in the name.
    /// </para>
    /// </example>
    [XmlAttribute("name")]
    public string Name { get; set; } = default!;

    [XmlElement("summary")]
    public SummaryNode Summary { get; set; } = default!;

    [XmlElement("remarks")]
    public RemarkNode Remarks { get; set; } = default!;

    [XmlElement("param", typeof(ParamNode))]
    public ParamNode[] Params { get; set; } = default!;

    [XmlElement("typeparam", typeof(TypeParamNode))]
    public TypeParamNode[] TypeParams { get; set; } = default!;

    [XmlElement("returns")]
    public ReturnsNode Returns { get; set; } = default!;

    [XmlElement("exception")]
    public ExceptionNode[] Exceptions { get; set; } = default!;

    [XmlElement("example", typeof(ExampleNode))]
    public ExampleNode[] Examples { get; set; } = default!;

    public IDString ID
    {
        get
        {
            return new IDString(this.Name);
        }
    }
}
