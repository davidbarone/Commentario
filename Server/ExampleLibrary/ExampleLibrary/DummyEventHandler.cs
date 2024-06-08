namespace ExampleLibrary;

/// <summary>
/// A dummy delegate (callback) event handler which specifies <see cref="DummyEventArgs"/> args. 
/// </summary>
/// <remarks>
/// See: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines
/// </remarks>
/// <param name="sender">The sender of the event.</param>
/// <param name="args">The args.</param>
public delegate void DummyDelegateHandler(object? sender, DummyEventArgs args);
