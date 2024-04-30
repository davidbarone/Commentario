using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class SimpleNode
{
    [XmlText]
    public string Text { get; set; }
}
public class CodeNode : SimpleNode { }

public class SummaryNode : SimpleNode { }

public class ReturnsNode : SimpleNode { }
