using System.Xml.Serialization;


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

[Serializable, XmlRoot(ElementName = "doc")]
[XmlType("doc")]
public class DocumentNode
{
    [XmlElement("assembly")]
    public AssemblyNode Assembly { get; set; }


    [XmlArray("members")]
    [XmlArrayItem("member", typeof(MemberNode))]
    public MemberNode[] Members { get; set; }

}

public class AssemblyNode
{
    [XmlElement("name")]
    public string Name { get; set; }
}

public class MemberNode
{

    [XmlAttribute("name")]
    public string Name { get; set; } 

    [XmlElement("summary")]
    public string Summary { get; set; }

    [XmlArrayItem("Param")]
    public ParamNode[] Params { get; set; }
}

public class ParamNode
{
    public string Name { get; set; }

    [XmlText]
    public string Description { get; set; }
}