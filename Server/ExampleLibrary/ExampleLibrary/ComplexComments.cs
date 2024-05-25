namespace ExampleLibrary;

public class ExceptionA(string message) : Exception(message) { }

public class ExceptionB(string message) : Exception(message) { }

/// <summary>
/// This class contains some members with more complex commentary.
/// </summary>
public class ComplexComments
{

    /// <summary>
    /// This is a method that returns nothing.
    /// </summary>
    /// <remarks>
    /// <para>This method returns nothing.</para>
    /// <example>
    /// This is an example of calling this method.
    /// <code>MethodThatHasRemarks();</code>
    /// </example>
    /// <para>Did we say, this method returns nothing.</para>
    /// </remarks>
    /// <returns>This method returns nothing.</returns>
    public string MethodThatHasRemarks()
    {
        return "";
    }

    /// <summary>
    /// This is a method that throws 2 possible exceptions.
    /// </summary>
    /// <param name="a">The first parameter.</param>
    /// <param name="b">The second parameter</param>
    /// <exception cref="ExceptionA">This error is thrown if a is less than b.</exception>
    /// <exception cref="ExceptionB">This error is thrown in all other cases.</exception>
    public void MethodThatThrowsTwoExceptionTypes(int a, int b)
    {
        if (a < b)
        {
            throw new ExceptionA("Whoops!");
        }
        else
        {
            throw new ExceptionB("Whoops!");
        }
    }
}