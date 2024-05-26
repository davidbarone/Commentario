using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// A description of an exception that can be thrown.
/// </summary>
public class ExceptionNode : ItemsNode
{
    /// <summary>
    /// The exception type name that is thrown.
    /// </summary>
    [XmlAttribute("cref")]
    public string Name { get; set; }
}