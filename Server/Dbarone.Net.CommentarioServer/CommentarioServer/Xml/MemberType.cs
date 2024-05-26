namespace Dbarone.CommentarioServer;

/// <summary>
/// The member type.
/// </summary>
public enum MemberType : byte
{
    /// <summary>
    /// The member is a namespace.
    /// </summary>
    Namespace,

    /// <summary>
    /// The member is a type.
    /// </summary>
    Type,

    /// <summary>
    /// The member is a field.
    /// </summary>
    Field,

    /// <summary>
    /// The member is a property.
    /// </summary>
    Property,

    /// <summary>
    /// The member is a method.
    /// </summary>
    Method,

    /// <summary>
    /// The member is an event.
    /// </summary>
    Event,

    /// <summary>
    /// The member is an error?
    /// </summary>
    ErrorString
}
