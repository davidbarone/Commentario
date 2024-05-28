namespace ExampleLibrary;

/// <summary>
/// A general-purpose test class that contains lots of various members to test documentation features.
/// </summary>
public class Test
{
    private int A { get; set; }
    private int B { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Test()
    {

    }

    /// <summary>
    /// Constructor with 1 parameter.
    /// </summary>
    /// <param name="a">First value.</param>
    public Test(int a)
    {
        A = a;
    }

    /// <summary>
    /// Constructor with 2 parameters.
    /// </summary>
    /// <param name="a">First value.</param>
    /// <param name="b">Second value.</param>
    public Test(int a, int b)
    {
        A = a;
        B = b;
    }

    /// <summary>
    /// This method takes 2 integers and adds them.
    /// </summary>
    /// <param name="a">The first integer.</param>
    /// <param name="b">The second integer.</param>
    /// <returns>Returns the sum of a and b.</returns>
    public int Add(int a, int b)
    {
        return a + b;
    }


    /// <summary>
    /// An example generic method.
    /// </summary>
    /// <typeparam name="T">The type parameter 'T'.</typeparam>
    /// <param name="input">The input value of type 'T'.</param>
    /// <returns>Returns the input.</returns>
    public T GenericMethod<T>(T input)
    {
        return input;
    }

    /// <summary>
    /// This function does nothing and doesn't return a value.
    /// </summary>
    public void NothingFunction()
    {

    }

    /// <summary>
    /// An example method with multiple parameters.
    /// </summary>
    /// <param name="intValue">An integer value.</param>
    /// <param name="dateTimeValue">A date time value.</param>
    /// <param name="guidValue">A guid value.</param>
    public void ParameterWithMultipleParameters(int intValue, DateTime dateTimeValue, Guid guidValue)
    {
    }

    /// <summary>
    /// This is a method that throws an exception.
    /// </summary>
    /// <exception cref="NotSupportedException">Throws a colourful exception.</exception>
    public void MethodThatRaisesException()
    {
        throw new NotSupportedException("This is not supported!");
    }

    /// <summary>
    /// This method has some examples.
    /// </summary>
    /// <example>
    /// This is example #1:
    /// <code>
    /// This is some code #1.
    /// </code>
    /// </example>
    /// <example>
    /// This is example #2.
    /// <code>
    /// This is some code #2.
    /// </code>
    /// </example>
    public void MethodWithExamples()
    {

    }

    /// <summary>
    /// This is a test boolean property.
    /// </summary>
    public bool SomeFlag { get; set; }

    /// <summary>
    /// A generic method.
    /// </summary>
    /// <typeparam name="TInput">The input type parameter.</typeparam>
    /// <typeparam name="TOutput">The output type parameter.</typeparam>
    /// <param name="input">The input value.</param>
    /// <param name="flag">A flag that can be set.</param>
    /// <returns>Returns a value of type TOutput.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public TOutput GenericMethod<TInput, TOutput>(TInput input, bool flag)
    {
        throw new NotImplementedException("This is not implemented");
    }
}