<h2>Summary</h2>
Commentario is a VSCode extension that documents .NET assemblies using a client-server architecture.

<h2>How Commentario Works</h2>
Documentation comes from 2 aspects:
<ul>
    <li>Reflecting over the assembly metadata</li>
    <li>Additional descriptive information provided by the xml comments / document file</li>
</ul>

<p>
The documentation does not require the addition xml comments file, but the documentation will be enhanced if it is present.
This tool does not generate the xml comments file. For that you will need to use another extension, for example the ms-dotnettools.csharp extension.
This extension will take your target assembly, reflect through the types and members, and merge the xml comments documentation to create a fully formatted
reference document. 
</p>

<p>
Commentario is able to format the following tags in the xml documentation:
<ul>
    <li>summary</li>
    <li>remark</li>
    <li>returns</li>
    <li>param</li>
    <li>exception</li>
    <li>para</li>
    <li>list</li>
    <li>c</li>
    <li>code</li>
    <li>example</li>
    <li>see</li>
    <li>typeparam</li>
</ul>
</p>

<h2>Generating Documentation</h2>
The code example below shows how to generate documentation:
<pre>
<code>
var assemblyPath = "";      // path to assembly
var outputPath = "";        // path to output document
var outputType = "html";    // html format
var allowOverwrite = true;  // set to true to overwrite output document if it exists
var xmlCommentsPath = "";   // path to xml comments file
var readMePath = "";        // path to readme file
var stylesPath = "";        // path to styles file

DocumentGenerator docGen = DocumentGenerator.Create(
    assemblyPath,
    outputPath,
    outputType,
    allowOverwrite,
    xmlCommentsPath,
    readMePath,
    stylesPath);

docGen.GenerateDocument();  // creates documentation
</code>
</pre>