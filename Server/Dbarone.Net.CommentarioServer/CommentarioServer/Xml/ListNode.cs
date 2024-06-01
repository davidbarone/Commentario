using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Used to create a list of items in a comment.
/// </summary>
public class ListNode
{
    /// <summary>
    /// The name of list. Can be bullet, number, or table.
    /// </summary>
    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlElement("listheader")]
    public ListItemNode ListHeader { get; set; }

    [XmlElement("item", typeof(ListItemNode))]
    public ListItemNode[] Items { get; set; }
}

public class ListItemNode
{

    [XmlElement("term")]
    public string Term { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }
}
