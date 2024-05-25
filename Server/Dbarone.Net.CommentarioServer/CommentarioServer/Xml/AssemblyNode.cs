using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Represents the top level assembly node in the documentation.
/// </summary>
public class AssemblyNode
{
    [XmlElement("name")]
    public string Name { get; set; }
}
