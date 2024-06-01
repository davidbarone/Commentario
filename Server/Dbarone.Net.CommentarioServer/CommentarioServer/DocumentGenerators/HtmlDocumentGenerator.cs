using System.Reflection;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Generates documentation in html format.
/// </summary>
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
                    commentText = RenderItems(commentSummary.Items);
                }
            }
            var str = $@"<tr><td><a href=""#{t.ToCommentId()}"">{t.Name}</a></td><td>{t.Namespace}</td><td>{commentText}</td></tr>";
            return str;
        }));

        if (types is null || types.Length == 0)
        {
            return "";
        }
        else
        {
            return @$"
<h3>{header}</h3>
<div class=""table"">
    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Namespace</th>
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
                            commentText = commentTypeParamsNode.Text ?? "";
                        }
                    }
                }
                var str = $@"<tr><td>{g}</a></td><td>{commentText}</td></tr>";
                return str;
            }));
            return @$"
<h3>Type Parameters</h3>
<div class=""table"">
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
                summary = RenderItems(summaryNode.Items);
            }
        }

        // Inherits
        var inherits = "";
        var inheritedType = this.GetInheritedType(type);
        if (inheritedType is not null)
        {
            inherits = @$"
<h3>Base Class</h3>
{inheritedType.Name}";
        }

        // Implements
        var implements = "";
        var implementedTypes = this.GetInterfacesImplemented(type);
        if (implementedTypes is not null && implementedTypes.Length > 0)
        {
            implements = @$"
<h3>Implemented Interfaces</h3>
<ul>{string.Join("", implementedTypes.Select(i => $"<li>{i.Name}</li>"))}</ul>";
        }

        var template = @$"

<div class=""type"">       
    <h2 id=""{type.ToCommentId()}"">{this.GetTypeCategory(type)}: {type.Name}</h2>
    <div class=""type-inner"">
        <a href=""#top"">Back to top</a>
        <h3>Definition:</h3>
        <ul>
            <li>Namespace: {type.Namespace}</li>
            <li>Assembly: {type.Assembly.FullName}</li>
        </ul>

        {inherits}

        {implements}

        <h3>Summary</h3>
        {summary}

        {this.RenderTypeGenericArguments(type)}
        {this.RenderTypeTOCSection(type, "Constructors", this.GetConstructors(type))}
        {this.RenderTypeTOCSection(type, "Fields", this.GetFields(type))}
        {this.RenderTypeTOCSection(type, "Properties", this.GetProperties(type))}
        {this.RenderTypeTOCSection(type, "Methods", this.GetMethods(type))}
        {this.RenderTypeTOCSection(type, "Events", this.GetEvents(type))}
        </div>
</div>
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
                    memberCommentText = RenderItems(memberNodeSummary.Items);
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
<h3>{header}</h3>
<div class=""table"">
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
        var memberType = member.GetMemberTypeName();

        // Get signature
        var signature = "";

        if (memberType == "Method")
        {
            signature = (member as MethodInfo).GetSignature(false);
        }
        else if (memberType == "Constructor")
        {
            signature = (member as ConstructorInfo).GetSignature(false);
        }

        if (!string.IsNullOrEmpty(signature))
        {
            signature = @$"
<h3>Signature</h3>
<pre><code>{signature}</code></pre>
            ";
        }

        // Summary
        var node = Comments.GetDocumentForMember(member);
        SummaryNode? summaryNode = null;
        string summary = "";

        if (node is not null)
        {
            summaryNode = node.Summary;
            if (summaryNode is not null)
            {
                summary = RenderItems(summaryNode.Items);
            }
        }

        // Remarks
        RemarkNode? remarksNode = null;
        string remarks = "";

        if (node is not null)
        {
            remarksNode = node.Remarks;
            if (remarksNode is not null)
            {
                remarks = $@"<h3>Remarks</h3>
{RenderItems(remarksNode.Items)}";
            }
        }

        // Examples (note these normally in remarks section, but can be placed in member)
        ExampleNode[]? exampleNodes = null;
        string examples = "";

        if (node is not null)
        {
            exampleNodes = node.Examples;
            if (exampleNodes is not null)
            {
                examples = $@"<h3>Examples</h3>
{string.Join("", exampleNodes.Select(e => RenderItems(e.Items)))}";
            }
        }

        var declaringType = member.DeclaringType!;

        // Parameters
        var parameters = "";
        var parameterInfos = this.GetMemberParameters(member);
        if (parameterInfos is not null && parameterInfos.Length > 0)
        {
            parameters = $@"
<h3>Parameters</h3>
<div class=""table"">
    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Type</th>
                <th>Description</th>
            </tr>
        </thead>
        <tbody>
            {string.Join("", parameterInfos.Select(
                    p =>
                    @$"
    <tr>
        <td>{p.Name}</td>
        <td>{p.ParameterType}</td>
        <td>{(node is not null && node.Params is not null ? this.RenderParam(node.Params.FirstOrDefault(n => n.Name.Equals(p.Name, StringComparison.Ordinal))) : "")}</td>
    </tr>"))}
        </tbody>
    </table>
</div>";
        }

        // Returns
        var returns = "";
        if (node is not null)
        {
            returns = this.RenderReturns(node.Returns);
        }

        // Exceptions
        var exceptions = "";
        if (node is not null)
        {
            exceptions = this.RenderExceptions(node.Exceptions);
        }

        var template = @$"
<div class=""member"">
    <h2 id=""{member.ToCommentId()}"">{member.GetMemberTypeName()}: {member.Name}</h1>
    <div class=""member-inner"">

        <h3>Declaring Type</h3>
        <a href=""#{declaringType.ToCommentId()}"">{member.DeclaringType}</a>

        {signature}

        <h3>Summary</h3>
        {summary}

        {remarks}

        {examples}

        {parameters}

        {returns}

        {exceptions}

    </div>
</div>
";
        return template;
    }


    protected override string RenderItems(object[] items)
    {
        List<string> results = new List<string>();
        foreach (var item in items)
        {
            if (item.GetType() == typeof(string))
            {
                results.Add((string)item);
            }
            else if (item.GetType() == typeof(ExampleNode))
            {
                results.Add(RenderExample((ExampleNode)item));
            }
            else if (item.GetType() == typeof(CNode))
            {
                results.Add(RenderC((CNode)item));
            }
            else if (item.GetType() == typeof(CodeNode))
            {
                results.Add(RenderCode((CodeNode)item));
            }
            else if (item.GetType() == typeof(ParaNode))
            {
                results.Add(RenderPara((ParaNode)item));
            }
            else if (item.GetType() == typeof(SeeNode))
            {
                results.Add(RenderSee((SeeNode)item));
            }
            else
            {
                results.Add(item.ToString());
            }
        }
        return string.Join("", results);
    }

    protected override string RenderExample(ExampleNode node)
    {
        return $@"<h3>Example</h3>
{this.RenderItems(node.Items)}";
    }

    protected override string RenderCode(CodeNode node)
    {
        return $@"<pre><code class=""csharp"">{node.Text}</code></pre>";
    }

    protected override string RenderC(CNode node)
    {
        return $"<pre><code>{node.Text}</code></pre>";
    }

    protected override string RenderPara(ParaNode node)
    {
        return $"<p>{this.RenderItems(node.Items)}</p>";
    }

    protected override string RenderSee(SeeNode node)
    {
        return $"<a href='{node.Member}'>{node.Text}</a>";
    }

    protected override string RenderReturns(ReturnsNode node)
    {
        var returns = "";
        if (node is not null && node.Items is not null)
        {
            returns = $@"<h3>Returns</h3>
{RenderItems(node.Items)}";
        }
        return returns;
    }

    protected override string RenderExceptions(ExceptionNode[] nodes)
    {
        var exceptions = "";
        if (nodes is not null && nodes.Length > 0)
        {
            exceptions = $@"<h3>Exceptions</h3>{string.Join("", nodes.Select(n => RenderException(n)))}";
        }
        return exceptions;
    }

    protected override string RenderException(ExceptionNode node)
    {
        var exception = "";
        if (node is not null)
        {
            exception = $@"<div><b>{node.Name}</b></div>";
            if (node.Items is not null)
            {
                exception += $@"{this.RenderItems(node.Items)}";
            }
        }
        return exception;
    }

    protected override string RenderParam(ParamNode? node)
    {
        if (node is null || node.Items is null) { return ""; }
        else
        {
            return this.RenderItems(node.Items);
        }
    }
}