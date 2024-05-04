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
        return this.Members.FirstOrDefault(m => m.ID.Name == typeId);
    }

    public MemberNode? GetDocumentForMember(MemberInfo member)
    {
        var memberName = member.ToString();

        // remove the return type which is in the member.ToString()
        var spacePos = memberName!.IndexOf(" ");
        if (spacePos > 0)
        {
            memberName = memberName.Substring(spacePos + 1);
        }

        // The xml comments exlude empty parentheses from methods, and have no spaces between parameters.
        // We need to make memberName consistent with this.
        memberName = memberName!.Replace("()", "").Replace(" ", "");

        return this.Members.FirstOrDefault(m => m.ID.MemberName == memberName);
    }
}
