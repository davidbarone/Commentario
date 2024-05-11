/* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using System.Reflection;

namespace Dbarone.Net.CommentarioServer;

public abstract class MethodBaseSignature
{
    /// <summary>
    /// Gets the method access modifier.
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/access-modifiers
    /// </summary>
    /// <param name="method">The method</param>
    /// <returns>Returns the access modifier.</returns>
    protected string GetAccessModifier(MethodBase method)
    {
        string modifier = "";

        if (method.IsAssembly)
        {
            modifier = "internal ";

            if (method.IsFamily)
                modifier += "protected ";
        }
        else if (method.IsPublic)
        {
            modifier = "public ";
        }
        else if (method.IsPrivate)
        {
            modifier = "private ";
        }
        else if (method.IsFamily)
        {
            modifier = "protected ";
        }

        if (method.IsStatic)
            modifier += "static ";

        return modifier;
    }

    public string BuildArguments(MethodBase method, bool invokable)
    {
        var isExtensionMethod = false;
        var methodInfo = method as MethodInfo; 
        
        if (methodInfo is not null) {
            isExtensionMethod = methodInfo.GetCustomAttributesData().Any(ca => ca.AttributeType == typeof(System.Runtime.CompilerServices.ExtensionAttribute));
        }

        //var isExtensionMethod = method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false);
        var methodParameters = method.GetParameters().AsEnumerable();

        // If this signature is designed to be invoked and it's an extension method
        if (isExtensionMethod && invokable)
        {
            // Skip the first argument
            methodParameters = methodParameters.Skip(1);
        }

        var methodParameterSignatures = methodParameters.Select(param =>
        {
            var signature = string.Empty;

            if (param.ParameterType.IsByRef)
                signature = "ref ";
            else if (param.IsOut)
                signature = "out ";
            else if (isExtensionMethod && param.Position == 0)
                signature = "this ";

            if (!invokable)
            {
                signature += TypeSignature.Build(param.ParameterType) + " ";
            }

            signature += param.Name;

            return signature;
        });

        var methodParameterString = "(" + string.Join(", ", methodParameterSignatures) + ")";

        return methodParameterString;
    }

    public string BuildGenerics(MethodBase method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));
        if (!method.IsGenericMethod) throw new ArgumentException($"{method.Name} is not generic.");

        return TypeSignature.BuildGenerics(method.GetGenericArguments());
    }
}
