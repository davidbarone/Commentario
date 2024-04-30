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
    /// The namespace name.
    /// </summary>
    public string Namespace { get; set; } = default!;

    /// <summary>
    /// The name of the parent type. Used for members and nested types within a type.
    /// </summary>
    public string Parent { get; set; } = default!;

    /// <summary>
    /// The name of the type or member.
    /// </summary>
    public string Name { get; set; } = default!;



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
        var fqnParts = this.FullyQualifiedName.Split('(');  // look for first '('. Required for Methods and properties with arguments.
        if (fqnParts.Length == 2)
        {
            this.Arguments = fqnParts[1];
            this.Arguments = this.Arguments.Substring(0, this.Arguments.Length - 1);    // remove last ')'
        }

        // Calculate namespace, parent, name
        var nameParts = fqnParts[0].Split('.');
        if (nameParts.Length >= 4)
        {
            throw new Exception($"Too many name parts for id: [{id}]: {nameParts.Length}.");
        }
        else if (nameParts.Length >= 3)
        {
            this.Namespace = nameParts[nameParts.Length - 3];
            this.Parent = nameParts[nameParts.Length - 2];
            this.Name = nameParts[nameParts.Length - 1];
        }
        else if (nameParts.Length >= 2)
        {
            this.Namespace = nameParts[nameParts.Length - 2];
            this.Parent = "";
            this.Name = nameParts[nameParts.Length - 1];
        }
        else
        {
            this.Namespace = "";
            this.Parent = "";
            this.Name = nameParts[nameParts.Length - 1];
        }

        if (!string.IsNullOrEmpty(this.Parent))
        {
            // Check for generic type arguments on parent. This will type be a type, so check for ` character.
            var arr = this.Parent.Split("`");
            this.Parent = arr[0];
            if (arr.Length == 2)
            {
                this.ParentTypeArguments = int.Parse(arr[1]);
            }
        }

        if (this.MemberType == MemberType.Type && !string.IsNullOrEmpty(this.Name))
        {
            var arr = this.Name.Split("`");
            this.Name = arr[0];
            if (arr.Length == 2)
            {
                this.TypeArguments = int.Parse(arr[1]);
            }
        }

        if ("FPEM".Contains(splits[0]) && !string.IsNullOrEmpty(this.Name))
        {
            var arr = this.Name.Split("``");
            this.Name = arr[0];
            if (arr.Length == 2)
            {
                this.TypeArguments = int.Parse(arr[1]);
            }
        }
    }
}
