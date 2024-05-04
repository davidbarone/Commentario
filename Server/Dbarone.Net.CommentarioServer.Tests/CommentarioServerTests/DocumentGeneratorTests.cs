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
    public void BasicTests()
    {
        string xmlCommentsPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.xml";
        string assemblyPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.dll";
        string readmePath = @"..\..\..\..\ExampleLibrary\readme.txt";
        string outputPath = @"..\..\..\..\..\ExampleLibrary.html";   // place in Commentario root folder, so gets checked into GitHub

        DocumentGenerator dg = DocumentGenerator.Create(xmlCommentsPath, assemblyPath, readmePath, outputPath, OutputType.Html);
        dg.GenerateDocument();
    }
}