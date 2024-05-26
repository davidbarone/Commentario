using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Allows you to specify a link from text.
/// </summary>
public class SeeNode : TextNode
{
    [XmlAttribute("cref")]
    public string Member { get; set; }
}