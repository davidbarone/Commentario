using System.Reflection;
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

    public MemberNode? GetDocumentForType(Type type)
    {
        var typeId = type.FullName;
        MemberNode empty = new MemberNode();
        return this.Members.FirstOrDefault(m => m.ID.Name == typeId);
    }

    public MemberNode? GetDocumentForMember(MemberInfo member)
    {
        var commentId = member.ToCommentId();
        MemberNode empty = new MemberNode();
        return this.Members.FirstOrDefault(m => m.ID.Id == commentId);
    }
}
