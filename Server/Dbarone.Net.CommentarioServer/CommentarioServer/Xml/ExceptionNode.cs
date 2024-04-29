using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class ExceptionNode
{

    [XmlAttribute("cref")]
    public string Name { get; set; }

    [XmlText]
    public string Description { get; set; }
}