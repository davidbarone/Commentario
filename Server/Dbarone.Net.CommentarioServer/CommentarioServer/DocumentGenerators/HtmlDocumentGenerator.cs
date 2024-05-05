using System.Reflection;
using Dbarone.CommentarioServer;
using Dbarone.Net.CommentarioServer;

public class HtmlDocumentGenerator : DocumentGenerator
{
    public HtmlDocumentGenerator(string xmlCommentsPath, string assemblyPath, string readMePath, string outputPath) : base(xmlCommentsPath, assemblyPath, readMePath, outputPath) { }

    protected override string RenderTOCSection(string header, Type[] types)
    {
        var values = string.Join("", types.OrderBy(t => t.Name).Select(t =>
        {
            var commentText = "";
            var commentMember = Comments.GetDocumentForType(t);
            if (commentMember is not null)
            {
                var commentSummary = commentMember.Summary;
                if (commentSummary is not null)
                {
                    commentText = commentSummary.Text;
                }
            }
            var str = $@"<tr><td><a href=""#{t.ToCommentId()}"">{t.Name}</a></td><td>{commentText}</td></tr>";
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
            <th>Name</th>
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

    protected override string RenderTypeGenericArguments(Type type)
    {
        var genericArgs = type.GetTypeGenericArguments();
        if (genericArgs.Length == 0)
        {
            return "";
        }
        else
        {
            // Create a table
            var values = string.Join("", genericArgs.Select(g =>
            {
                var commentText = "";
                var commentMember = Comments.GetDocumentForType(type);
                if (commentMember is not null)
                {
                    var commentTypeParams = commentMember.TypeParams;
                    if (commentTypeParams is not null)
                    {
                        var commentTypeParamsNode = commentTypeParams.FirstOrDefault(p => p.Name.Equals(g));
                        if (commentTypeParamsNode is not null)
                        {
                            commentText = commentTypeParamsNode.Description ?? "";
                        }
                    }
                }
                var str = $@"<tr><td>{g}</a></td><td>{commentText}</td></tr>";
                return str;
            }));
            return @$"
<h2>Type Parameters</h2>
<div>
<table>
    <thead>
        <tr>
            <th>Name</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        {values}
    </tbody>
</table>
</div>";
        }
    }


    protected override string RenderType(Type type)
    {
        var node = Comments.GetDocumentForType(type);
        SummaryNode? summaryNode = null;
        string summary = "";

        if (node is not null)
        {
            summaryNode = node.Summary;
            if (summaryNode is not null)
            {
                summary = summaryNode.Text;
            }
        }

        var template = @$"
<h1 id=""{type.ToCommentId()}"">{type.Name} {this.GetTypeCategory(type)}</h1>
<a href=""#top"">Back to top</a>
<h2>Definition:</h2>
<ul>
    <li>Namespace: {type.Namespace}</li>
    <li>Assembly: {type.Assembly.FullName}</li>
</ul>

<h2>Summary</h2>
{summary}

{this.RenderTypeGenericArguments(type)}
{this.RenderTypeTOCSection(type, "Constructors", this.GetConstructors(type))}
{this.RenderTypeTOCSection(type, "Fields", this.GetFields(type))}
{this.RenderTypeTOCSection(type, "Properties", this.GetProperties(type))}
{this.RenderTypeTOCSection(type, "Methods", this.GetMethods(type))}
{this.RenderTypeTOCSection(type, "Events", this.GetEvents(type))}

<hr />
";
        return template;
    }

    protected override string RenderTypeTOCSection(Type type, string header, MemberInfo[] members)
    {
        var values = string.Join("", members.OrderBy(m => m.Name).Select(m =>
        {
            var memberCommentText = "";
            var memberNode = Comments.GetDocumentForMember(m);
            if (memberNode is not null)
            {
                var memberNodeSummary = memberNode.Summary;
                if (memberNodeSummary is not null)
                {
                    memberCommentText = memberNodeSummary.Text;
                }
            }
            return $@"<tr><td><a href=""#{m.ToCommentId()}"">{m.ToString()}</a></td><td>{memberCommentText}</td></tr>";
        }));

        if (members is null || members.Length == 0)
        {
            return "";
        }
        else
        {
            return @$"
<div>
<h2>{header}</h2>
<table>
    <thead>
        <tr>
            <th>Name</th>
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
        // Get type of member (Property, Method, Field etc.)
        var memberType = member.GetType().Name.Replace("Info", "");

        var declaringType = member.DeclaringType!;

        var template = @$"
<h1 id=""{member.ToCommentId()}"">{member.Name} {member.GetMemberTypeName()}</h1>
<a href=""#{declaringType.ToCommentId()}"">Back to parent</a>
<h2>Definition:</h2>

Parameters

Returns

Exceptions

<hr />
";
        return template;
    }
}