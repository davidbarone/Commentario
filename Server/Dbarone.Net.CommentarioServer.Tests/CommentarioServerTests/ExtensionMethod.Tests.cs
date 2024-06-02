using Xunit;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Dbarone.Net.CommentarioServer.Tests;

/// <summary>
/// Test class for testing type parameters
/// </summary>
/// <typeparam name="TInput">The input type</typeparam>
/// <typeparam name="TOutput">The output type</typeparam>
class TestGenericClass<TInput, TOutput>
{

}

public class ExtensionMethodTests
{

    [Theory]
    [InlineData(typeof(DocumentNode), "T:Dbarone.Net.CommentarioServer.DocumentNode")]
    public void TestTypeToCommentId(Type type, string expectedCommentId)
    {
        var actualCommentId = type.ToCommentId();
        Assert.Equal(expectedCommentId, actualCommentId);
    }

    public static IEnumerable<object[]> TestMemberData()
    {
        Type type = typeof(DocumentNode);
        MethodInfo miGetDocumentForType = type.GetMethod("GetDocumentForType")!;
        PropertyInfo piAssembly = type.GetProperty("Assembly");
        yield return new object[] { miGetDocumentForType, "M:Dbarone.Net.CommentarioServer.DocumentNode.GetDocumentForType(System.Type)" };
        yield return new object[] { piAssembly, "P:Dbarone.Net.CommentarioServer.DocumentNode.Assembly" };
    }

    [Theory]
    [MemberData(nameof(TestMemberData))]
    public void TestMemberToCommentId(MemberInfo member, string expectedCommentId)
    {
        var actualCommentId = member.ToCommentId();
        Assert.Equal(expectedCommentId, actualCommentId);
    }

    [Fact]
    public void TestGetTypeGenericArguments()
    {
        var actual = typeof(TestGenericClass<,>).GetTypeGenericArguments();
        Assert.Equal(new string[] { "TInput", "TOutput" }, actual);
    }

    public static IEnumerable<object[]> CommentIdMemberData =>
            new List<object[]>
            {
            new object[] {typeof(DocumentGenerator).GetMember("Create").First(), "M:Dbarone.Net.CommentarioServer.DocumentGenerator.Create(System.String,System.String,Dbarone.Net.CommentarioServer.OutputType,Boolean,System.String,System.String,System.String)" }
            };

    [Theory, MemberData(nameof(CommentIdMemberData))]
    public void TestMemberCommentId(MemberInfo member, string expectedCommentId)
    {
        var actualCommentId = member.ToCommentId();
        Assert.Equal(expectedCommentId, actualCommentId);
    }
}