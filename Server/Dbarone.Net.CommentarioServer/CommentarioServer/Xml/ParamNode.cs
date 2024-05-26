using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Comment for a parameter on a method. The items property stores the descriptions.
/// </summary>
public class ParamNode : ItemsNode
{
    /// <summary>
    /// The name of the parameters.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }
}
