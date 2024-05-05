using System.Reflection;
using Dbarone.CommentarioServer;

namespace Dbarone.Net.CommentarioServer;

/// <summary>
/// Useful extension methods. Primarily to provide reflection services.
/// </summary>
public static class ExtensionMethods
{
    public static string GetMemberType(this MemberInfo member)
    {
        var memberType = "";
        if (member is PropertyInfo) { memberType = "P"; }
        else if (member is MethodInfo) { memberType = "M"; }
        else if (member is ConstructorInfo) { memberType = "M"; }
        else if (member is FieldInfo) { memberType = "F"; }
        else if (member is EventInfo) { memberType = "E"; }
        return memberType;
    }

    public static string GetMemberTypeName(this MemberInfo member)
    {
        var memberType = "";
        if (member is PropertyInfo) { memberType = "Property"; }
        else if (member is MethodInfo) { memberType = "Method"; }
        else if (member is ConstructorInfo) { memberType = "Constructor"; }
        else if (member is FieldInfo) { memberType = "Field"; }
        else if (member is EventInfo) { memberType = "Event"; }
        return memberType;
    }

    /// <summary>
    /// Returns a comment id string for a given type member. Can be used to locate the comments from xml comments file. 
    /// </summary>
    /// <param name="member">The MemberInfo object.</param>
    /// <returns>Returns a comment id string.</returns>
    public static string ToCommentId(this MemberInfo member)
    {
        var memberName = member.ToString();
        Type declaringType = member.DeclaringType!;

        // remove the return type which is in the member.ToString()
        var spacePos = memberName!.IndexOf(" ");
        if (spacePos > 0)
        {
            memberName = memberName.Substring(spacePos + 1);
        }

        // '.' in name is replaced with '#'. Only replace in the name part (not the arguments).
        var splits = memberName.Split("(");
        splits[0] = splits[0].Replace(".", "#");
        memberName = string.Join("(", splits);

        // Add the namespace / parent type
        memberName = $"{declaringType.FullName}.{memberName}";

        // The xml comments exlude empty parentheses from methods, and have no spaces between parameters.
        // We need to make memberName consistent with this.
        memberName = memberName!.Replace("()", "").Replace(" ", "");
        return $"{member.GetMemberType()}:{memberName}";
    }

    /// <summary>
    /// Returns a comment id string for a type which can be used to locate the type comments from the xml comments file
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ToCommentId(this Type type)
    {
        return $"T:{type.FullName}";
    }

    /// <summary>
    /// Gets the type letter arguments for a generic type.
    /// </summary>
    /// <param name="type">A type. Must be a generic type.</param>
    /// <returns>Returns an array of type parameter names.</returns>
    public static string[] GetTypeGenericArguments(this Type type)
    {
        var typeArgs = type.GetGenericArguments();
        return typeArgs.Select(t => t.Name).ToArray();
    }
}