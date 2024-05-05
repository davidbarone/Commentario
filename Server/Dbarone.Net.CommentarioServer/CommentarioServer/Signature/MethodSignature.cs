/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dbarone.Net.CommentarioServer;

public class MethodSignature : MethodBaseSignature
{
    public string Build(MethodInfo method, bool invokable)
    {
        var signatureBuilder = new StringBuilder();

        // Add our method accessors if it's not invokable
        if (!invokable)
        {
            signatureBuilder.Append(GetAccessModifier(method));
            signatureBuilder.Append(TypeSignature.Build(method.ReturnType));
            signatureBuilder.Append(" ");
        }

        // Add method name
        signatureBuilder.Append(method.Name);

        // Add method generics
        if (method.IsGenericMethod)
        {
            signatureBuilder.Append(BuildGenerics(method));
        }

        // Add method parameters
        signatureBuilder.Append(BuildArguments(method, invokable));

        return signatureBuilder.ToString();
    }
}
