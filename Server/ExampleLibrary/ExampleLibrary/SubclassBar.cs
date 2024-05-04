namespace ExampleLibrary;

/// <summary>
/// A class that implements an interface.
/// </summary>
public class SubclassBar : ITestInterface
{
    /// <summary>
    /// The type of the object (foo).
    /// </summary>
    public FooBarBazEnum FooBarBazType { get; set; } = FooBarBazEnum.BAR;
}