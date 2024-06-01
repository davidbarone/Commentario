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
        <name>
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
        <name>
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
            Assert.Equal("The type parameter 'T'.", reader.Document.Members.First().TypeParams.First().Text);
            Assert.Equal("a", reader.Document.Members.First().Params.First().Name);
            Assert.Equal("First param.", reader.Document.Members.First().Params.First().Items[0].ToString());
            Assert.Equal("Return value.", reader.Document.Members.First().Returns.Items[0]);
        }
    }

    [Fact]
    public void LoadCommentsWithSingleMemberCommentWithExamples()
    {
        var xmlText = @"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>
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
    public void LoadCommentsWithSingleMemberCommentWithRemark()
    {
        var xmlText = @"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>
            ExampleLibrary
        </name>
    </assembly>
    <members>
        <member name=""M:ExampleLibrary.Test.GenericMethod()"">
            <summary>A method.</summary>
            <remarks>
                <para>First paragraph.</para>
                <para>Second paragraph.</para>
                <code>This is some code.</code>
                This is some plain text.
                <see cref=""test""/>
                This is some more text.
            </remarks>
        </member>
    </members>
</doc>";
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
        {
            XmlCommentsReader reader = new XmlCommentsReader(ms);
            Assert.Single(reader.Document.Members);
            Assert.Equal("M:ExampleLibrary.Test.GenericMethod()", reader.Document.Members.First().Name);
            // above comments have 6 children in the remarks section.
            Assert.Equal(6, reader.Document.Members.First().Remarks.Items.Count());

            // Check the code node
            var codeNode = reader.Document.Members.First().Remarks.Items[2];
            Assert.IsType<CodeNode>(codeNode);
            Assert.Equal("This is some code.", (codeNode as CodeNode)!.Text);
        }
    }

   [Fact]
    public void LoadCommentsWithSingleMemberCommentWithList()
    {
        var xmlText = @"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>
            ExampleLibrary
        </name>
    </assembly>
    <members>
        <member name=""M:ExampleLibrary.Test.GenericMethod()"">
            <summary>A method.</summary>
            <remarks>
                <list type=""table"">
                    <listheader>
                        <term>Item</term>
                        <description>Description</description>
                    </listheader>
                    <item>
                        <term>A</term>
                        <description>Item A</description>
                    </item>
                    <item>
                        <term>B</term>
                        <description>Item B</description>
                    </item>
                    <item>
                        <term>C</term>
                        <description>Item C</description>
                    </item>
                </list>
            </remarks>
        </member>
    </members>
</doc>";
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlText)))
        {
            XmlCommentsReader reader = new XmlCommentsReader(ms);
            Assert.Single(reader.Document.Members);
            var list = reader.Document.Members.First().Remarks.Items.First();
            Assert.IsType<ListNode>(list);
            Assert.Equal(3, (list as ListNode)!.Items.Count());
            Assert.Equal("Item A", (list as ListNode)!.Items[0].Description);
        }
    }

    [Fact]
    public void TestXmlCommentsParsing()
    {
        string xmlCommentsPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.xml";

        XmlCommentsReader reader = new XmlCommentsReader(xmlCommentsPath);
    }
}