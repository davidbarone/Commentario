using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class TypeParamNode
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlText]
    public string Description { get; set; }
}
