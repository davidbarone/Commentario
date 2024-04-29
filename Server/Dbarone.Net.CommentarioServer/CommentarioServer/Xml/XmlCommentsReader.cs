using System.Xml.Serialization;

namespace Dbarone.Net.CommentarioServer;

public class XmlCommentsReader
{
    public DocumentNode Document { get; set; }
    public XmlCommentsReader(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(DocumentNode));

        using (Stream reader = new FileStream(path, FileMode.Open))
        {
            // Call the Deserialize method to restore the object's state.
            this.Document = (DocumentNode)serializer.Deserialize(reader);
        }
    }
}




