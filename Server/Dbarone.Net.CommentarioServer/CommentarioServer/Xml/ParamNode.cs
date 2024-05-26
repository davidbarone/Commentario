using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Comment for a parameter on a method.
/// </summary>
public class ParamNode
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlText(typeof(string))]
    [XmlElement("see", typeof(SeeNode))]
    public object[] Items { get; set; }
}
