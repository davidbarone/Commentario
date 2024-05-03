using Dbarone.Net.CommentarioServer;

public class HtmlDocumentGenerator : DocumentGenerator
{
    public HtmlDocumentGenerator(string xmlCommentsPath, string assemblyPath, string outputPath) : base(xmlCommentsPath, assemblyPath, outputPath) { }

    protected override string RenderTOC(Type[] types)
    {
        var values = string.Join("", types.OrderBy(t => t.Name).Select(t => $"<tr><td>{t.Name}</td><td></td></tr>"));

        var template = @$"
<div class=""overflow-auto"">
<table class=""striped"">
    <thead>
        <tr>
            <th>Class</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        {values}
    </tbody>
</table>
</div>
        ";

        return template;
    }

    protected override string RenderType(Type type)
    {
        var template = @$"
<h1>Class: {type.Name}</h1>
<h2>Definition:</h2>
<ul>
    <li>Namespace: {type.Namespace}</li>
    <li>Assembly: {type.Assembly.FullName}</li>
</ul>

<h2>Constructors:</h2>

<h2>Properties</h2>

<h2>Methods</h2>

<h2>Events</h2>

<hr />
";
        return template;
    }
}