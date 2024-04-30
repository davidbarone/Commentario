using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class ExampleNode
{
    [XmlText(typeof(string))]
    [XmlElement("code", typeof(CodeNode))]
    public object[] Items { get; set; }
}
