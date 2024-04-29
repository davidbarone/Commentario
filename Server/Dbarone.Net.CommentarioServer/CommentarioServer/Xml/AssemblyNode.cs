using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class AssemblyNode
{
    [XmlElement("name")]
    public string Name { get; set; }
}
