using Dbarone.CommentarioServer;
using Xunit;

public class IDStringTests
{

    [Theory]
    [InlineData("T:Color", "", "", "Color", 0, 0)]             // simple type
    [InlineData("T:Acme.IProcess", "Acme", "", "IProcess", 0, 0)]     // type + namespace
    [InlineData("T:Acme.ValueType", "Acme", "", "ValueType", 0, 0)]    // type + namespace
    [InlineData("T:Acme.Widget", "Acme", "", "Widget", 0, 0)]       // type + namespace
    [InlineData("T:Acme.Widget.NestedClass", "Acme", "Widget", "NestedClass", 0, 0)]   // nested type
    [InlineData("T:Acme.Widget.IMenuItem", "Acme", "Widget", "IMenuItem", 0, 0)]     // nested type
    [InlineData("T:Acme.Widget.Del", "Acme", "Widget", "Del", 0, 0)]
    [InlineData("T:Acme.Widget.Direction", "Acme", "Widget", "Direction", 0, 0)]
    [InlineData("T:Acme.MyList`1", "Acme", "", "MyList", 0, 1)]
    [InlineData("T:Acme.MyList`1.Helper`2", "Acme", "MyList", "Helper", 1, 2)]
    public void TypeStringTests(string id, string expectedNamespace, string expectedParent, string expectedName, int expectedParentTypeParameters, int expectedTypeParameters)
    {
        var idString = new IDString(id);
        Assert.Equal(expectedNamespace, idString.Namespace);
        Assert.Equal(expectedParent, idString.Parent);
        Assert.Equal(expectedName, idString.Name);
        Assert.Equal(expectedParentTypeParameters, idString.ParentTypeArguments);
        Assert.Equal(expectedTypeParameters, idString.TypeArguments);
    }
}