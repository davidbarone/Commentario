namespace Dbarone.CommentarioServer;

/// <summary>
/// The member type.
/// </summary>
public enum MemberType : byte
{
    Namespace,
    Type,
    Field,
    Property,
    Method,
    Event,
    ErrorString
}
