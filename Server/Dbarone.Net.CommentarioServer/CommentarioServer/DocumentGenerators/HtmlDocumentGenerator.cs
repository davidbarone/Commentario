using System.Reflection;
using Dbarone.Net.CommentarioServer;

public class HtmlDocumentGenerator : DocumentGenerator
{
    public HtmlDocumentGenerator(string xmlCommentsPath, string assemblyPath, string readMePath, string outputPath) : base(xmlCommentsPath, assemblyPath, readMePath, outputPath) { }

    protected override string RenderTOCSection(string header, Type[] types, DocumentNode comments)
    {
        var values = string.Join("", types.OrderBy(t => t.Name).Select(t =>
        {
            var commentText = "";
            var commentMember = comments.GetDocumentForType(t);
            if (commentMember is not null)
            {
                var commentSummary = commentMember.Summary;
                if (commentSummary is not null)
                {
                    commentText = commentSummary.Text;
                }
            }
            var str = $@"<tr><td><a href=""#{t.FullName}"">{t.Name}</a></td><td>{commentText}</td></tr>";
            return str;
        }));

        if (types is null || types.Length == 0)
        {
            return "";
        }
        else
        {
            return @$"
<h2>{header}</h2>
<div>
<table>
    <thead>
        <tr>
            <th>Type</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        {values}
    </tbody>
</table>
</div>
        ";
        }
    }

    protected override string RenderType(Type type)
    {
        var template = @$"
<h1 id=""{type.FullName}"">{type.Name} {this.GetTypeCategory(type)}</h1>
<a href=""#top"">Back to top</a>
<h2>Definition:</h2>
<ul>
    <li>Namespace: {type.Namespace}</li>
    <li>Assembly: {type.Assembly.FullName}</li>
</ul>

<h2>Constructors:</h2>
{this.RenderTypeTOCSection(type, this.GetConstructors(type))}

<h2>Properties</h2>
{this.RenderTypeTOCSection(type, this.GetProperties(type))}

<h2>Methods</h2>
{this.RenderTypeTOCSection(type, this.GetMethods(type))}

<h2>Events</h2>
{this.RenderTypeTOCSection(type, this.GetEvents(type))}

<hr />
";
        return template;
    }

    protected override string RenderTypeTOCSection(Type type, MemberInfo[] members)
    {
        var values = string.Join("", members.OrderBy(t => t.Name).Select(t => $"<tr><td>{t.Name}</td><td></td></tr>"));

        if (members is null || members.Length == 0)
        {
            return "";
        }
        else
        {
            return @$"
<div>
<table>
    <thead>
        <tr>
            <th>Type</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        {values}
    </tbody>
</table>
</div>
        ";
        }
    }

    protected override string RenderTypeMember(MemberInfo member)
    {
        return "";
    }
}