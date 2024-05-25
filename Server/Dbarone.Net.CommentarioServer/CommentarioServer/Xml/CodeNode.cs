using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class SimpleNode
{
    [XmlText(typeof(string))]
    public string Text { get; set; }
}
public class CodeNode : SimpleNode { }

/// <summary>
/// Every class and member should have a one sentence summary describing its purpose.
/// </summary>
public class SummaryNode
{
    [XmlText(typeof(string))]
    [XmlElement("see", typeof(SeeNode))]
    public object[] Items { get; set; }
}

public class ReturnsNode
{
    [XmlText(typeof(string))]
    [XmlElement("see", typeof(SeeNode))]
    public object[] Items { get; set; }

}
