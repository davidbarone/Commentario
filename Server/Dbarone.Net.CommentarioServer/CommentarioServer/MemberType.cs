namespace Dbarone.CommentarioServer;

/// <summary>
/// The member type.
/// </summary>
internal enum MemberType : byte
{
    Namespace,
    Type,
    Field,
    Property,
    Method,
    Event,
    ErrorString
}
