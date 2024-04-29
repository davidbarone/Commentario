using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

[Serializable, XmlRoot(ElementName = "doc")]
[XmlType("doc")]
public class DocumentNode
{
    [XmlElement("assembly")]
    public AssemblyNode Assembly { get; set; }


    [XmlArray("members")]
    [XmlArrayItem("member", typeof(MemberNode))]
    public MemberNode[] Members { get; set; }

}
