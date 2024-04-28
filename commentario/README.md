# commentario README

## Features

This extension creates formatted documentation from C# xml comments.

## Known Issues

None

## Release Notes

Users appreciate release notes as you update your extension.

### 1.0.0

Initial release of Commentario

### 1.0.1

---

## Following extension guidelines

Ensure that you've read through the extensions guidelines and follow the best practices for creating your extension.

* [Extension Guidelines](https://code.visualstudio.com/api/references/extension-guidelines)

## Working with Markdown

You can author your README using Visual Studio Code. Here are some useful editor keyboard shortcuts:

* Split the editor (`Cmd+\` on macOS or `Ctrl+\` on Windows and Linux).
* Toggle preview (`Shift+Cmd+V` on macOS or `Shift+Ctrl+V` on Windows and Linux).
* Press `Ctrl+Space` (Windows, Linux, macOS) to see a list of Markdown snippets.

## For more information

* [Visual Studio Code's Markdown Support](http://code.visualstudio.com/docs/languages/markdown)
* [Markdown Syntax Reference](https://help.github.com/articles/markdown-basics/)

**Enjoy!**


# Creating Solution + Projects + References via solution file using dotnet / VSCode

dotnet new sln
dotnet new classlib -o Dbarone.Net.Extensions.String
dotnet new xunit -o Dbarone.Net.Extensions.String.Tests
dotnet sln add Dbarone.Net.Extensions.String
dotnet sln add Dbarone.Net.Extensions.String.Tests
dotnet add Dbarone.Net.Extensions.String.Tests reference Dbarone.Net.Extensions.String

https://markheath.net/post/multiple-nuget-single-repo

# Creating Unit Tests

https://xunit.net/docs/getting-started/netfx/visual-studio

# To test
dotnet test

# To build
dotnet build

# To pack
- Add package metadata (.nuspec metadata) to csproj file
- https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-pack
- https://docs.microsoft.com/en-us/nuget/reference/nuspec
- https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target

# To push to Nuget
- https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-nuget-push
- dotnet nuget push Dbarone.Net.Extensions.String.1.0.0.nupkg --api-key xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx -s https://api.nuget.org/v3/index.json

# Unlisting / Deleting a nuget package
- https://docs.microsoft.com/en-us/nuget/nuget-org/policies/deleting-packages

# Package Versioning
- https://theroks.com/nuget-package-versioning/

