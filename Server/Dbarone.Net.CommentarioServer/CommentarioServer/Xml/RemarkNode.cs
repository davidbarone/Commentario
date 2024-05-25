using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Expands on the summary sentence to provide more information for readers.
/// </summary>
public class RemarkNode
{
    [XmlText(typeof(string))]
    [XmlElement("code", typeof(CodeNode))]
    [XmlElement("para", typeof(ParaNode))]
    [XmlElement("c", typeof(CNode))]
    [XmlElement("see", typeof(SeeNode))]
    [XmlElement("example", typeof(ExampleNode))]
    public object[] Items { get; set; }
}


public class ParaNode
{
    [XmlText(typeof(string))]
    [XmlElement("code", typeof(CodeNode))]
    [XmlElement("para", typeof(ParaNode))]
    [XmlElement("c", typeof(CNode))]
    [XmlElement("see", typeof(SeeNode))]
    public object[] Items { get; set; }
}

/// <summary>
/// Denotes text within a description should be marked as single-line code.
/// </summary>
public class CNode : SimpleNode
{

}

/// <summary>
/// Allows you to specify a link from text
/// </summary>
public class SeeNode
{
    [XmlAttribute("cref")]
    public string Member { get; set; }

    [XmlText]
    public string Description { get; set; }
}