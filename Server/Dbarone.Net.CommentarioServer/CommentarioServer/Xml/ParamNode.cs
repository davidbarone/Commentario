using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class ParamNode
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlText]
    public string Description { get; set; }
}
