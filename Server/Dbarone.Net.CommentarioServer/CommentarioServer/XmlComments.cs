using System.Xml.Linq;

/// <summary>
/// Class to process the xml comments file
/// </summary>
public class XmlComments {
    string Path { get; set; }
    XDocument Document { get; set; } 
    public XmlComments(string path) {
        this.Path = path;
        this.Document = XDocument.Load(path);
    }


}