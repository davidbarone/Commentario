# Commentario
Commentario is a VS Code extension that formats C# xml comments. I created this project to learn more about VS code internals, and also as I have a number of other projects that I would like to maintain decent documentation for.

## Getting Started With VS Code Extension Development
Like most new projects, a bit of reading was required to get started with VSCode extension authoring. Some useful starting points were:
- https://www.digitalocean.com/community/tutorials/how-to-create-your-first-visual-studio-code-extension
- https://code.visualstudio.com/api/get-started/your-first-extension

VSCode extensions are generally written using JavaScript or TypeScript, and there is a rich amount of functionality provided through the `vscode` JavaScript API to let you interact with various components of VSCode like the editor.

One issue I encountered immediately, was that the extension I am trying to build needs to reflect through an assembly's metadata to build the documentation. This requires a .NET module and so I needed to look at non-node solution. I've built this as a simple client-server. All the documentation is contained in a .NET class library, that has a very thin console application wrapper. This console app when executed, will reflect through the target assembly metadata, and create documentation using an additional xml comments file to supplement the documentation.

The client part of the extension, simply takes the configuration values set in the workspace settings, and executes the console app.

### Set up of Client
The client project was set up using Yeoman: `npx --package yo --package generator-code -- yo code`

![setup](https://raw.githubusercontent.com/davidbarone/Commentario/main/Images/setup.bmp)

## Configuration
The extension uses the following contribution configuration points:
- `Commentario.assemblyPath`: The path to the target assembly you want documented.
- `Commentario.outputPath`: The path to the output documentation file.
- `Commentario.xmlCommentsPath`: The path to an optional C# xml comments file.
- `Commentario.readMePath`: The path to an optional readme file. The contents will be included at the start of the documentation file. The contents of this file need to be in html format.
- `Commentario.outputType`: The output document format type. Currently only html format is supported.
- `Commentario.debugMode`: If set to true, the documentation will include warnings where xml comments are missing.

## Commands
This extension adds a single contribution comment:
- `commentario.createDocumentation`: This command will create the documentation file.

## Running the Client
The client can be tested by running the `Run Extension` launch configuration 

## Publishing the Extension
The following article explains how to publish an extension: https://code.visualstudio.com/api/working-with-extensions/publishing-extension.
Before publishing, you'll need to compile `Dbarone.Net.CommentarioConsole` in release mode, and copy the output files into the `/out/server` folder of the client extension project.

The extension can be easily built using `vsce package`.


## Example
For an example of the output documentation, refer to: [ExampleLibrary.html](https://html-preview.github.io/?url=https://github.com/davidbarone/Commentario/blob/main/ExampleLibrary.html).
