using System.IO;
using Dbarone.Net.CommentarioServer;
using Xunit;
using System;

/// <summary>
/// Document generator tests. 
/// </summary>
[Collection("Sequential")]
public class DocumentGeneratorTests
{

    [Fact]
    public void CreateExampleDocumentation()
    {
        string xmlCommentsPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.xml";
        string assemblyPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.dll";
        string readmePath = @"..\..\..\..\ExampleLibrary\readme.txt";
        string outputPath = @"..\..\..\..\..\ExampleLibrary.html";   // place in Commentario root folder, so gets checked into GitHub

        DocumentGenerator dg = DocumentGenerator.Create(
            assemblyPath,
            outputPath,
            OutputType.html,
            xmlCommentsPath, readmePath);
        dg.GenerateDocument();
    }

    [Fact]
    public void CreateCommentarioDocumentation()
    {
        string xmlCommentsPath = @"..\..\..\..\Dbarone.Net.CommentarioServer\bin\Debug\net8.0\Dbarone.Net.CommentarioServer.xml";
        string assemblyPath = @"..\..\..\..\Dbarone.Net.CommentarioServer\bin\Debug\net8.0\Dbarone.Net.CommentarioServer.dll";
        string readmePath = @"..\..\..\..\Dbarone.Net.CommentarioServer\readme.txt";
        string outputPath = @"..\..\..\..\..\Dbarone.Net.CommentarioServer.html";   // place in Commentario root folder, so gets checked into GitHub

        DocumentGenerator dg = DocumentGenerator.Create(
            assemblyPath,
            outputPath,
            OutputType.html,
            xmlCommentsPath,
            readmePath);
        dg.GenerateDocument();

    }
}