namespace ExampleLibrary;

/// <summary>
/// A class that implements an interface.
/// </summary>
public class SubclassFoo : ITestInterface
{
    /// <summary>
    /// The type of the object (foo).
    /// </summary>
    public FooBarBazEnum FooBarBazType { get; set; } = FooBarBazEnum.FOO;
}