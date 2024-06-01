using System.Reflection;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Generates documentation in html format. Html implementation of <see cref="DocumentGenerator"/>.
/// </summary>
public class HtmlDocumentGenerator : DocumentGenerator
{
    public HtmlDocumentGenerator(string xmlCommentsPath, string assemblyPath, string readMePath, string outputPath) : base(xmlCommentsPath, assemblyPath, readMePath, outputPath) { }


    protected override string RenderDocument()
    {
        var contentTypes = "";
        var contentMembers = "";

        foreach (var type in this.GetTypes())
        {
            contentTypes += RenderType(type);
            foreach (var member in GetMembers(type))
            {
                contentMembers += RenderTypeMember(member);
            }
        }

        var template = @$"
<!DOCTYPE html>
<html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <meta http-equiv=""X-UA-Compatible"" content=""ie=edge"">
        <title>HTML 5 Boilerplate</title>

        {this.GetCSSStyles()}

        <!-- https://highlightjs.org/#usage -->
        <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/default.min.css"">
        <script src=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/highlight.min.js""></script>
        <!-- and it's easy to individually load additional languages -->
        <script src=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/languages/csharp.min.js""></script>
    </head>

    <body id=""top"">

        <h1>{GetAssembly()}</h1>
        {this.GetReadMe()}

        <div class=""toc"">
            <h2>Table of Contents</h2>
            <div class=""toc-inner"">
                {this.RenderTOCSection("Classes", this.GetClasses())}
                {this.RenderTOCSection("Structs", this.GetStructs())}
                {this.RenderTOCSection("Interfaces", this.GetInterfaces())}
                {this.RenderTOCSection("Enums", this.GetEnums())}
            </div>
        </div>

        {contentTypes}
        {contentMembers}

    </body>
    <!-- https://highlightjs.org/#usage -->
    <script>hljs.highlightAll();</script>
</html>";

        return template;
    }

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
</div>";
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
                summary = @$"
<h3>Summary</h3>
{RenderItems(summaryNode.Items)}";
            }
        }

        var definition = @$"
        <h3>Definition:</h3>
        <ul>
            <li>Namespace: {type.Namespace}</li>
            <li>Assembly: {type.Assembly.FullName}</li>
        </ul>
        ";

        // Inherits
        var inherits = "";
        var inheritedType = this.GetInheritedType(type);
        if (inheritedType is not null)
        {
            inherits = @$"
<h3>Base Class</h3>
{inheritedType.Name}";
        }

        // Subclasses
        var subclasses = "";
        var subclassTypes = this.GetSubClasses(type);
        if (subclassTypes is not null && subclassTypes.Length > 0)
        {
            subclasses = @$"
<h3>Sub Classes</h3>
<ul>{string.Join("", subclassTypes.Select(s => $"<li>{s.Name}</li>"))}</ul>";
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

        {definition}
        
        {inherits}

        {implements}

        {subclasses}

        {summary}

        {this.RenderTypeGenericArguments(type)}
        {this.RenderTypeTOCSection(type, "Constructors", this.GetConstructors(type))}
        {this.RenderTypeTOCSection(type, "Fields", this.GetFields(type))}
        {this.RenderTypeTOCSection(type, "Properties", this.GetProperties(type))}
        {this.RenderTypeTOCSection(type, "Methods", this.GetMethods(type))}
        {this.RenderTypeTOCSection(type, "Events", this.GetEvents(type))}
    </div>
</div>";
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
</div>";
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
        <td>{GetLinkForType(p.ParameterType)}</td>
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
        if (items is null)
        {
            return "";
        }

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
            else if (item.GetType() == typeof(ListNode))
            {
                results.Add(RenderList((ListNode)item));
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
        // Need to remove extra tab characters - these are introduced by the xml comments formatter
        var lines = node
            .Text
            .Split(Environment.NewLine)
            .Select(l => (l[0] == '\t') ? l.Substring(1) : l);

        // remove first and list rows.
        if (lines.Count() >= 2 && string.IsNullOrEmpty(lines.First()) && string.IsNullOrEmpty(lines.Last()))
        {
            lines = lines
                .Skip(1)
                .Take(lines.Count() - 2);
        }

        return $@"<pre><code class=""csharp"">{string.Join(Environment.NewLine, lines)}</code></pre>";
    }

    protected override string RenderC(CNode node)
    {
        return $"<code>{node.Text}</code>";
    }

    protected override string RenderPara(ParaNode node)
    {
        return $"<p>{this.RenderItems(node.Items)}</p>";
    }

    /// <summary>
    /// Renders a see link.
    /// </summary>
    /// <remarks>
    /// The inner text value is optional. If not set, then the type name will be determined from the cref value.
    /// <param name="node">The <see cref="SeeNode"/> instance.</param>
    /// <returns>Returns a hyperlink to the type.</returns>
    protected override string RenderSee(SeeNode node)
    {
        var idString = new IDString(node.Member);
        var typeName = node.Text ?? idString.TypeName;

        return $@"<a href=""{node.Member}"">{typeName}</a>";
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

    protected override string RenderList(ListNode node)
    {
        // currently always renders as table
        var str = @$"
<div class=""table"">
    <table>
        <tr>
            <th>{node.ListHeader.Term}</th>
            <th>{node.ListHeader.Description}</th>
        </tr>
        {string.Join("", node.Items.Select(i => $"<tr><td><b>{i.Term}</b></td><td>{i.Description}</td></tr>"))}
    </table>
</div>";
        return str;
    }
}