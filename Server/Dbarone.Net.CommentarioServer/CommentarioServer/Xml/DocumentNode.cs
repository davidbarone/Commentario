using System.Reflection;
using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Represents the root node of the xml comments file.
/// </summary>
[Serializable, XmlRoot(ElementName = "doc")]
[XmlType("doc")]
public class DocumentNode
{
    /// <summary>
    /// The assembly node. Provides information about the assembly.
    /// </summary>
    [XmlElement("assembly")]
    public AssemblyNode Assembly { get; set; }

    /// <summary>
    /// The collection of members. All documentation nodes whether for Type documentation or Member documentation are included in this collection.
    /// </summary>
    [XmlArray("members")]
    [XmlArrayItem("member", typeof(MemberNode))]
    public MemberNode[] Members { get; set; } = new List<MemberNode>().ToArray();

    /// <summary>
    /// Gets the xml comment node for a specific type.
    /// </summary>
    /// <param name="type">The type to get document for.</param>
    /// <returns>Returns the document node for the type.</returns>
    public MemberNode? GetDocumentForType(Type type)
    {
        var typeId = type.FullName;
        MemberNode empty = new MemberNode();
        return this.Members.FirstOrDefault(m => m.ID.Name == typeId);
    }

    /// <summary>
    /// Gets the xml comment node for the specific member.
    /// </summary>
    /// <param name="member">The member to get the document for.</param>
    /// <returns>Returns the document node for the member.</returns>
    public MemberNode? GetDocumentForMember(MemberInfo member)
    {
        var commentId = member.ToCommentId();
        MemberNode empty = new MemberNode();
        return this.Members.FirstOrDefault(m => m.ID.Id == commentId);
    }
}
