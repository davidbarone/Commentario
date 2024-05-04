namespace ExampleLibrary;

/// <summary>
/// A test interface. 
/// </summary>
public interface ITestInterface
{
    /// <summary>
    /// The type of the object (foo, bar, baz).
    /// </summary>
    FooBarBazEnum FooBarBazType { get; set; }
}