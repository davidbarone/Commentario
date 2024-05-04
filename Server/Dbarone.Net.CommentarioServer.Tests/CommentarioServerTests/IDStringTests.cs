using System.Reflection.Metadata.Ecma335;
using Dbarone.CommentarioServer;
using Dbarone.Net.CommentarioServer;
using Xunit;

/// <summary>
/// A generic type.
/// </summary>
/// <typeparam name="T">The type parameter, 'T'.</typeparam>
public class GenericType<T>
{

}
public class IDStringTests
{

    [Theory]
    [InlineData("T:Color", "Color")]             // simple type
    [InlineData("T:Acme.IProcess", "Acme.IProcess")]     // type + namespace
    [InlineData("T:Acme.ValueType", "Acme.ValueType")]    // type + namespace
    [InlineData("T:Acme.Widget", "Acme.Widget")]       // type + namespace
    [InlineData("T:Acme.Widget.NestedClass", "Acme.Widget.NestedClass")]   // nested type
    [InlineData("T:Acme.Widget.IMenuItem", "Acme.Widget.IMenuItem")]     // nested type
    [InlineData("T:Acme.Widget.Del", "Acme.Widget.Del")]
    [InlineData("T:Acme.Widget.Direction", "Acme.Widget.Direction")]
    [InlineData("T:Acme.MyList`1", "Acme.MyList`1")]
    [InlineData("T:Acme.MyList`1.Helper`2", "Acme.MyList`1.Helper`2")]
    public void TypeStringTests(string id, string expectedName)
    {
        var idString = new IDString(id);
        Assert.Equal(expectedName, idString.Name);
    }
}