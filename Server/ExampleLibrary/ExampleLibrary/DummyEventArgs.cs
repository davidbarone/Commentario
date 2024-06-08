namespace ExampleLibrary;

/// <summary>
/// A dummy event args object.
/// </summary>
/// <remarks>
/// See: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines
/// </remarks>
public class DummyEventArgs : EventArgs
{
    /// <summary>
    /// The event id.
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// This is a useful message about the event.
    /// </summary>
    public string EventMessage { get; set; } = default!;

}