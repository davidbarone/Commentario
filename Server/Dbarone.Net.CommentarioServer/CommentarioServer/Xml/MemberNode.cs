using System.Xml.Serialization;
using Dbarone.CommentarioServer;

namespace Dbarone.Net.CommentarioServer;

public class MemberNode
{
    [XmlAttribute("name")]
    public string Name { get; set; } = default!;

    [XmlElement("summary")]
    public SummaryNode Summary { get; set; } = default!;

    [XmlElement("param", typeof(ParamNode))]
    public ParamNode[] Params { get; set; } = default!;

    [XmlElement("typeparam", typeof(TypeParamNode))]
    public TypeParamNode[] TypeParams { get; set; } = default!;

    [XmlElement("returns")]
    public ReturnsNode Returns { get; set; } = default!;

    [XmlElement("exception")]
    public ExceptionNode Exception { get; set; } = default!;

    [XmlElement("example")]
    public ExampleNode[] Examples { get; set; } = default!;

    public IDString ID
    {
        get
        {
            return new IDString(this.Name);
        }
    }
}
