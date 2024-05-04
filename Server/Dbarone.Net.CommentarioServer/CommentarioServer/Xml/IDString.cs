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
public class IDString
{
    /// <summary>
    /// The full member id string. In format [MemberType]:[memberid]
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// The id value after the initial ':' character.
    /// </summary>
    public string FullyQualifiedName { get; set; } = default!;

    /// <summary>
    /// The member type.
    /// </summary>
    public MemberType MemberType { get; set; } = MemberType.ErrorString;

    /// <summary>
    /// The name of the type or member. This optionally includes the namespace and the parent / enclosing type name. 
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// The member name. Only applies for members.
    /// </summary>
    public string MemberName { get; set; } = default!;

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
    /// constructor for IDString class.
    /// </summary>
    /// <param name="id">The ID string.</param>
    public IDString(string id)
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
        var bracketPos = this.FullyQualifiedName.IndexOf("(");

        if (bracketPos > 0)
        {
            this.Arguments = this.FullyQualifiedName.Substring(bracketPos);
            this.Name = this.FullyQualifiedName.Substring(0, bracketPos);

            // Get member name
            if ("FPEM".Contains(splits[0]))
            {
                var memberName = this.Name.Split(".").Reverse().First();
                // Xml comments replace '.' in names with '#'
                this.MemberName = $"{memberName}{this.Arguments}".Replace("#", ".");
            }
        }
        else
        {
            this.Arguments = "";
            this.Name = this.FullyQualifiedName;

            // Get member name
            if ("FPEM".Contains(splits[0]))
            {
                var memberName = this.Name.Split(".").Reverse().First();
                // Xml comments replace '.' in names with '#'
                this.MemberName = $"{memberName}{this.Arguments}".Replace("#", ".");
            }
        }
    }
}
