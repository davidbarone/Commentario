using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class ExceptionNode
{

    [XmlAttribute("cref")]
    public string Name { get; set; }

    [XmlText(typeof(string))]
    [XmlElement("see", typeof(SeeNode))]
    public object[] Items { get; set; }
}