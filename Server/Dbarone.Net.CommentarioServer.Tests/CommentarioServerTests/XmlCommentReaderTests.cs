using Xunit;

namespace Dbarone.Net.CommentarioServer.Tests;

public class XmlCOmmentReaderTests {
    
    [Fact]
    public void TestXmlCommentsParsing() {
        string xmlCommentsPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.xml";

        XmlCommentsReader reader = new XmlCommentsReader(xmlCommentsPath);
    }
}