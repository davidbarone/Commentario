using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// A text node represents a node in the xml comments that only has inner text.
/// This text is not processed any further, and the literal value is used as is. 
/// </summary>
public class TextNode
{
    /// <summary>
    /// The inner text value. This text is not processed any further.
    /// </summary>
    [XmlText(typeof(string))]
    public string Text { get; set; }
}