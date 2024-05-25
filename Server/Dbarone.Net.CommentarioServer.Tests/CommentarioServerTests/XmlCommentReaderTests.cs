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
    public void LoadCommentsWithSingleMemberCommentFull()
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
            <typeparam name=""T"">The type parameter 'T'.</typeparam>
            <param name=""a"">First param.</param>
            <returns>Return value.</returns>
            <exception cref=""T:System.NotSupportedException"">Throws a colourful exception.</exception>
        </member>
    </members>
</doc>";
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
        {
            XmlCommentsReader reader = new XmlCommentsReader(ms);
            Assert.Single(reader.Document.Members);
            Assert.Equal("M:ExampleLibrary.Test.GenericMethod()", reader.Document.Members.First().Name);
            Assert.Equal("T", reader.Document.Members.First().TypeParams.First().Name);
            Assert.Equal("The type parameter 'T'.", reader.Document.Members.First().TypeParams.First().Description);
            Assert.Equal("a", reader.Document.Members.First().Params.First().Name);
            Assert.Equal("First param.", reader.Document.Members.First().Params.First().Description);
            Assert.Equal("Return value.", reader.Document.Members.First().Returns.Text);
        }
    }

    [Fact]
    public void LoadCommentsWithSingleMemberCommentWithExamples()
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
            <example>
                This is example #1:
                <code>
                This is some code #1.
                </code>
            </example>
            <example>
                This is example #2.
                <code>
                This is some code #2.
                </code>
            </example>
        </member>
    </members>
</doc>";
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
        {
            XmlCommentsReader reader = new XmlCommentsReader(ms);
            Assert.Single(reader.Document.Members);
            Assert.Equal("M:ExampleLibrary.Test.GenericMethod()", reader.Document.Members.First().Name);
            Assert.Equal(2, reader.Document.Members.First().Examples.Count());
            Assert.Equal(2, reader.Document.Members.First().Examples.First().Items.Count());
            Assert.IsType<string>(reader.Document.Members.First().Examples.First().Items.First());
            Assert.IsType<CodeNode>(reader.Document.Members.First().Examples.First().Items.Last());
        }
    }

    [Fact]
    public void TestXmlCommentsParsing()
    {
        string xmlCommentsPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.xml";

        XmlCommentsReader reader = new XmlCommentsReader(xmlCommentsPath);
    }
}