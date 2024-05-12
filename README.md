# Commentario
A VS Code extension to format C# xml comments.

I had two goals when creating this project:
- Create an API documentation tool
- Learn how to create CSVode extensions

As far as an API documentation goes, I've got a number of libraries that I maintain, and I've always used the C# xml commenting to comment my code. Although I'm aware of a number of document formatting tools like Sandcastle and Doxygen.

Sandcastle https://ewsoftware.github.io/SHFB/html/bd1ddb51-1c4f-434f-bb1a-ce2135d3a909.htm is a really old tool. I remember dabbling with it around 2008. To be honest, given its age, I've not looked at it in recent years

Doxygen https://www.doxygen.nl/index.html looks like a useful tool. It appears to be a modern tool with nice output. I've not investigated this tool much either.

I guess the main reason for this project is to understand VSCode extension building, and the documentation use case is purely an excuse to learn.

## Getting Started

Like most new projects, a bit of reading is required to get started with VSCode extension authoring. Some examples I used were:
- https://www.digitalocean.com/community/tutorials/how-to-create-your-first-visual-studio-code-extension
- https://code.visualstudio.com/api/get-started/your-first-extension

VSCode extensions are generally written using Node, and there is a rich amount of functionality provided through the `vscode` JavaScript API to let you interact with various components of VSCode like the editor.

One issue I encountered immediately, was that the extension I am trying to build needs to reflect through an assembly's metadata to build the help. Therefore, this requires a .NET module.







## Example
For an example of the output documentation, refer to: [ExampleLibrary.html](https://html-preview.github.io/?url=https://github.com/davidbarone/Commentario/blob/main/ExampleLibrary.html).
