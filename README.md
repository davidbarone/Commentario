# Commentario
Commentario is a VS Code extension that formats C# xml comments. I created this project to learn more about VS code internals, and also as I have a number of other projects that I would like to maintain decent documentation for.

## Workspace Structure
The Commentario workspace is set up as follows:
- **client**: This is the VSCode extension. It's written in TypeScript.
- **server/Dbarone.Net.CommentarioConsole**: This is the VSCode server. It does all the actual work, and is called by the client. The console app is a very thin wrapper around Dbarone.Net.CommentarioServer.
- **server/Dbarone.Net.CommentarioServer**: The library contains the actual documentation code used by the console app.
- **server/Dbarone.Net.CommentarioServer.Tests**: A test library to test the documentation code.
- **Server/ExampleLibrary**: A small assembly used to test the various types of object allowed in an assembly that can be documented.

## Dbarone.Net.CommentarioServer
This project is the workhorse of commentario. All the documentation logic occurs here. In order to test, I've created a small project called `ExampleLibrary`. The purpose of this dummy project is to provide an example of each documentation type possible.

### Testing Dbarone.Net.CommentarioServer
A test project `Dbarone.Net.CommentatioServer.Tests` contains a number of test methods. You can execute this by running `dotnet test` from the `./server` folder. Alternatively, there's a launch configuration: `C#: Run Tests` which you can run.

## Getting Started With VS Code Extension Development
Like most new projects, a bit of reading was required to get started with VSCode extension authoring. Some useful starting points were:
- https://www.digitalocean.com/community/tutorials/how-to-create-your-first-visual-studio-code-extension
- https://code.visualstudio.com/api/get-started/your-first-extension

VSCode extensions are generally written using JavaScript or TypeScript, and there is a rich amount of functionality provided through the `vscode` JavaScript API to let you interact with various components of VSCode like the editor.

One issue I encountered immediately, was that the extension I am trying to build needs to reflect through an assembly's metadata to build the documentation. This requires a .NET module and so I needed to look at non-JavaScript solution. I've built this as a client-server solution. All the documentation functionality is contained in a .NET class library, that has a very thin console application wrapper. This console application when executed, will reflect through the target assembly metadata, and create documentation using an additional xml comments file to supplement the documentation.

The client part of the extension, simply takes the configuration values set in the workspace settings, and executes the console app.

**Note that this assembly will install binary files into your local VSCode extensions folder, and will be disabled for workspaces that are opened in restricted mode.**

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
This extension adds a single contribution command:
- `commentario.createDocumentation`: This command will create the documentation file.

## Running the Client
The client can be tested by running the `Run Extension` launch configuration 

## Publishing the Extension
The following article explains how to publish an extension: https://code.visualstudio.com/api/working-with-extensions/publishing-extension.
Before publishing, you'll need to compile `Dbarone.Net.CommentarioConsole` in release mode, and copy the output files into the `/out/server` folder of the client extension project.

The extension can be easily built using `vsce package`.

### Getting a Personal Access Token
To publish to the marketplace, I had to get a personal access token:

- https://learn.microsoft.com/en-gb/azure/devops/organizations/accounts/create-organization?view=azure-devops

## Testing the published extension.
Once the .vsix file is created, you can easily add it into your extensions. There is an `Install from VSIX...` option you can use. This allows the extension to be installed locally, rather than from the extension marketplace.

## Example
For an example of the output documentation, refer to: [ExampleLibrary.html](https://html-preview.github.io/?url=https://github.com/davidbarone/Commentario/blob/main/ExampleLibrary.html).
