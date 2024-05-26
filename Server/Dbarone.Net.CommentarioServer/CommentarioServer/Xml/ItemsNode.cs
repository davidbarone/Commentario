using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// An items node is a node that can contain multiple types of child node.
/// </summary>
public class ItemsNode
{
    /// <summary>
    /// The array of child item nodes. Multiple types of child item are permitted.
    /// </summary>
    [XmlText(typeof(string))]
    [XmlElement("code", typeof(CodeNode))]
    [XmlElement("para", typeof(ParaNode))]
    [XmlElement("c", typeof(CNode))]
    [XmlElement("see", typeof(SeeNode))]
    [XmlElement("example", typeof(ExampleNode))]
    public object[] Items { get; set; } = new object[] { };
}