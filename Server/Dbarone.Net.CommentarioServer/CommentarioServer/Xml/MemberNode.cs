using System.Xml.Serialization;
using Dbarone.CommentarioServer;

namespace Dbarone.Net.CommentarioServer;

public class MemberNode
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlElement("summary")]
    public SummaryNode Summary { get; set; }

    [XmlElement("param", typeof(ParamNode))]
    public ParamNode[] Params { get; set; }

    [XmlElement("typeparam", typeof(TypeParamNode))]
    public TypeParamNode[] TypeParams { get; set; }

    [XmlElement("returns")]
    public ReturnsNode Returns { get; set; }

    [XmlElement("exception")]
    public ExceptionNode Exception { get; set; }

    [XmlElement("example")]
    public ExampleNode[] Examples { get; set; }

    public IDString ID
    {
        get
        {
            return new IDString(this.Name);
        }
    }
}
