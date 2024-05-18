# Commentario README

Commentario is an extension for creating formatted documentation for .NET / C# assemblies. The documentation is built by reflecting over
a target assembly. An optional xml comments file can be used to create enhanced documentation.

## Features

Commentario creates html documentation for a .NET assembly.Currently only html format is supported - an example of the output is shown below:

![Example](./example.png)

The extension adds a single command contribution point:
- `commentario.createDocumentation`: Creates the documentation.

## Requirements

The extension uses a .NET server process to parse your assemblies. This assembly will be disabled in restricted mode.

## Extension Settings

The following configuration contribution extension points are defined. You will need to set these appropriately.
* `Commentario.assemblyPath`: The path to your assembly.
* `Commentario.outputPath`: The output path for the documentation file.
* `Commentario.xmlCommentsPath`: The optional path for the xml comments file.
* `Commentario.readMePath`: The optional path for a global readme file for the assembly. Contents must be html.
* `Commentario.outputType`: The output type of the document. Currently only 'html' is supported.
* `Commentario.debugMode`: Set to true for the output documentation to include debug information.

## Known Issues

This extension is currently in development, and is not recommended for any production use cases.

## Release Notes

Users appreciate release notes as you update your extension.

### 0.0.1

Initial release of Commentario
