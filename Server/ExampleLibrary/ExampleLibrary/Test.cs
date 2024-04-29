/// <summary>
/// A test class.
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
    public void MethodThatRaisesException() {
        throw new NotSupportedException("This is not supported!");
    }
}