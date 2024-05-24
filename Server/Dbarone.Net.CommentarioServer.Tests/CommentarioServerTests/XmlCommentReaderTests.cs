using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Dbarone.Net.CommentarioServer.Tests;

[Collection("Sequential")]
public class XmlCommentReaderTests
{
    [Fact]
    public void LoadCommentsWithNoMemberComments()
    {
        var xmlText = @"<?xml version=""1.0""?><doc><assembly><name>ExampleLibrary</name></assembly></doc>";
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
        {
            XmlCommentsReader reader = new XmlCommentsReader(ms);
            Assert.Empty(reader.Document.Members);
        }
    }

    [Fact]
    public void LoadCommentsWithSingleMemberComment()
    {
        var xmlText = @"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>f
            ExampleLibrary
        </name>
    </assembly>
    <members>
        <member name=""M:ExampleLibrary.Test.GenericMethod()"">
            <summary>A method.</summary>
        </member>
    </members>
</doc>";
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
        {
            XmlCommentsReader reader = new XmlCommentsReader(ms);
            Assert.Single(reader.Document.Members);
            Assert.Equal("M:ExampleLibrary.Test.GenericMethod()", reader.Document.Members.First().Name);
        }
    }


    [Fact]
    public void TestXmlCommentsParsing()
    {
        string xmlCommentsPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.xml";

        XmlCommentsReader reader = new XmlCommentsReader(xmlCommentsPath);
    }
}