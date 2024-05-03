using System.IO;
using Dbarone.Net.CommentarioServer;
using Xunit;
using System;

/// <summary>
/// Document generator tests. 
/// </summary>
public class DocumentGeneratorTests {

    [Fact]
    public void BasicTests() {
        string xmlCommentsPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.xml";
        string assemblyPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.dll";
        string outputPath = @"..\..\..\..\ExampleLibrary\bin\Debug\net8.0\ExampleLibrary.html";

        DocumentGenerator dg = DocumentGenerator.Create(xmlCommentsPath, assemblyPath, outputPath, OutputType.Html);
        dg.GenerateDocument();
    }
}