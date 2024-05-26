using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Represents a type parameter comment.
/// </summary>
public class TypeParamNode : TextNode
{
    /// <summary>
    /// The name of the type parameter.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }
}
