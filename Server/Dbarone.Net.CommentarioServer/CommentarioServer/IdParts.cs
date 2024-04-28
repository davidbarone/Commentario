using System;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Dynamic;

namespace Dbarone.CommentarioServer;

/// <summary>
/// Represents the parts making up a comment document id, known as the 'ID string'.
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>
internal class IdParts
{
    /// <summary>
    /// The full member id string. In format [MemberType]:[memberid]
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// The member type.
    /// </summary>
    public MemberType MemberType { get; set; } = MemberType.ErrorString;

    /// <summary>
    /// The id value after the initial ':' character.
    /// </summary>
    public string FullyQualifiedName { get; set; } = default!;

    /// <summary>
    /// Unique link tag based on fully qualified name. Used for generaing links for TOC etc.
    /// </summary>
    public string FullyQualifiedNameLink { get; set; } = default!;

    /// <summary>
    /// The number of generic type arguments on the parent type / class. 
    /// </summary>
    public int ParentTypeArguments { get; set; } = default!;

    /// <summary>
    /// The number of generic type arguments on the current member. 
    /// </summary>
    public int TypeArguments { get; set; } = default!;

    /// <summary>
    /// Arguments (if exist for methods and properties).
    /// </summary>
    public string Arguments { get; set; } = "";

    /// <summary>
    /// The namespace name.
    /// </summary>
    public string Namespace { get; set; } = default!;

    /// <summary>
    /// The name of the parent.
    /// </summary>
    public string Parent { get; set; } = default!;

    /// <summary>
    /// The name of the member.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// constructor for IdParts class.
    /// </summary>
    /// <param name="id">The ID string.</param>
    public IdParts(string id)
    {
        Console.WriteLine($"Processing: {id}...");
        this.Id = id;
        var splits = id.Split(':');
        switch (splits[0])
        {
            case "N": this.MemberType = MemberType.Namespace; break;
            case "F": this.MemberType = MemberType.Field; break;
            case "P": this.MemberType = MemberType.Property; break;
            case "T": this.MemberType = MemberType.Type; break;
            case "E": this.MemberType = MemberType.Event; break;
            case "M": this.MemberType = MemberType.Method; break;
            case "!": this.MemberType = MemberType.ErrorString; break;
            default: this.MemberType = MemberType.ErrorString; break;
        }
        this.FullyQualifiedName = splits[1];
        this.FullyQualifiedNameLink = this.FullyQualifiedName.Replace(".", "").ToLower();
        var fqnParts = this.FullyQualifiedName.Split('(');  // look for first '('. Required for Methods and properties with arguments.
        if (fqnParts.Length == 2)
        {
            this.Arguments = fqnParts[1];
            this.Arguments = this.Arguments.Substring(0, this.Arguments.Length - 1);    // remove last ')'
        }
        var nameParts = fqnParts[0].Split('.');
        this.Name = nameParts[nameParts.Length - 1];

        // Check name for generics. (contains '``' characters)
        var nameGenericSplits = this.Name.Split("``");
        this.Name = nameGenericSplits[0];
        if (nameGenericSplits.Length == 2)
        {
            this.TypeArguments = int.Parse(nameGenericSplits[1]);
        }

        if ("T" == splits[0])
        {
            // check type member for generics
            nameGenericSplits = this.Name.Split("`");
            this.Name = nameGenericSplits[0];
            if (nameGenericSplits.Length == 2)
            {
                this.TypeArguments = int.Parse(nameGenericSplits[1]);
            }
        }

        if ("FPEM".Contains(splits[0]))
        {
            // For fields, properties, events, methods, the
            // parent == containing type.
            this.Parent = nameParts[nameParts.Length - 2];
            this.Namespace = string.Join('.', nameParts.Take(nameParts.Length - 2));

            // Check type for generics (single '`' character)
            var parentGenericsSplits = this.Parent.Split('`');
            this.Parent = parentGenericsSplits[0];
            if (parentGenericsSplits.Length == 2)
            {
                this.ParentTypeArguments = int.Parse(parentGenericsSplits[1]);
            }
        }
        this.Namespace = string.Join('.', nameParts.Take(nameParts.Length - 1));
    }
}
