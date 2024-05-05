/* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Dbarone.Net.CommentarioServer;

public static class TestExtensionMethods
{
    public static string ExtensionMethod(this string firstParam, bool secondParam)
    {
        throw new NotImplementedException();
    }
}

public class SignatureExample {
    public SignatureExample(int param)
    {
    }

    public SignatureExample()
    {
    }

    protected SignatureExample(int param1, int param2)
    {
    }

    private SignatureExample(int param1, int param2, int param3)
    {
    }

    internal static void TestMethod5()
    {
    }

    public string TestMethod(string firstParam)
    {
        throw new NotImplementedException();
    }

    public List<string> TestMethod2<T>(string firstParam, T secondParam)
    {
        throw new NotImplementedException();
    }

    public void TestMethod3(System.Action<System.Action<System.Action<string>>> firstParam)
    {
        throw new NotImplementedException();
    }

    public void TestMethod4(Nullable<int> firstParam)
    {
        throw new NotImplementedException();
    }
}

public class SignatureTests
{

    [Fact]
    public void Example_Signature()
    {
        var type = typeof(SignatureExample);
        var method = type.GetMethod(nameof(SignatureExample.TestMethod));

        var signature = method.GetSignature(false);

        Assert.Equal("public string TestMethod(string firstParam)", signature);
    }

    [Fact]
    public void Example_Signature_Accessor()
    {
        var type = typeof(SignatureExample);
        var method = type.GetMethod(nameof(SignatureExample.TestMethod5), BindingFlags.Static | BindingFlags.NonPublic);

        var signature = method.GetSignature(false);

        Assert.Equal("internal static void TestMethod5()", signature);
    }

    [Fact]
    public void Example_Signature_Constructor()
    {
        var type = typeof(SignatureExample);
        var method = type.GetConstructors((BindingFlags)~0).Single(ctor => ctor.GetParameters().Count() == 1);

        var signature = method.GetSignature(false);

        Assert.Equal("public SignatureExample(int param)", signature);
    }

    [Fact]
    public void Example_Signature_Constructor_WithPrivateAccessor()
    {
        var type = typeof(SignatureExample);
        var method = type.GetConstructors((BindingFlags)~0).Single(ctor => ctor.GetParameters().Count() == 3);

        var signature = method.GetSignature(false);

        Assert.Equal("private SignatureExample(int param1, int param2, int param3)", signature);
    }

    [Fact]
    public void Example_Signature_Constructor_WithProtectedAccessor()
    {
        var type = typeof(SignatureExample);
        var method = type.GetConstructors((BindingFlags)~0).Single(ctor => ctor.GetParameters().Count() == 2);

        var signature = method.GetSignature(false);

        Assert.Equal("protected SignatureExample(int param1, int param2)", signature);
    }

    [Fact]
    public void Example_Signature_ExtensionMethod()
    {
        var type = typeof(TestExtensionMethods);
        var method = type.GetMethod(nameof(TestExtensionMethods.ExtensionMethod));

        var signature = method.GetSignature(false);

        Assert.Equal("public static string ExtensionMethod(this string firstParam, bool secondParam)", signature);
    }

    [Fact]
    public void Example_Signature_ExtensionMethod_Invokable()
    {
        var type = typeof(TestExtensionMethods);
        var method = type.GetMethod(nameof(TestExtensionMethods.ExtensionMethod));

        var signature = method.GetSignature(true);

        Assert.Equal("ExtensionMethod(secondParam)", signature);
    }

    [Fact]
    public void Example_Signature_Generics()
    {
        var type = typeof(SignatureExample);
        var method = type.GetMethod(nameof(SignatureExample.TestMethod2));

        var signature = method.GetSignature(false);

        Assert.Equal("public System.Collections.Generic.List<string> TestMethod2<T>(string firstParam, T secondParam)", signature);
    }

    [Fact]
    public void Example_Signature_Generics_Invokable()
    {
        var type = typeof(SignatureExample);
        var method = type.GetMethod(nameof(SignatureExample.TestMethod2));

        var signature = method.GetSignature(true);

        Assert.Equal("TestMethod2<T>(firstParam, secondParam)", signature);
    }

    [Fact]
    public void Example_Signature_Generics_Nested()
    {
        var type = typeof(SignatureExample);
        var method = type.GetMethod(nameof(SignatureExample.TestMethod3));

        var signature = method.GetSignature(false);

        Assert.Equal("public void TestMethod3(System.Action<System.Action<System.Action<string>>> firstParam)", signature);
    }

    [Fact]
    public void Example_Signature_Invokable()
    {
        var type = typeof(SignatureExample);
        var method = type.GetMethod(nameof(SignatureExample.TestMethod));

        var signature = method.GetSignature(true);

        Assert.Equal("TestMethod(firstParam)", signature);
    }

    [Fact]
    public void Example_Signature_Nullable()
    {
        var type = typeof(SignatureExample);
        var method = type.GetMethod(nameof(SignatureExample.TestMethod4));

        var signature = method.GetSignature(false);

        Assert.Equal("public void TestMethod4(int? firstParam)", signature);
    }
}
