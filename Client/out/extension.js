"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.deactivate = exports.activate = void 0;
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
const vscode = __importStar(require("vscode"));
const cp = require('child_process');
// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
function activate(context) {
    // Use the console to output diagnostic information (console.log) and errors (console.error)
    // This line of code will only be executed once when your extension is activated
    console.log('Congratulations, your extension "commentario" is now active!');
    // The command has been defined in the package.json file
    // Now provide the implementation of the command with registerCommand
    // The commandId parameter must match the command field in package.json
    let disposable = vscode.commands.registerCommand('commentario.createDocumentation', () => {
        // The code you place here will be executed every time your command is executed
        // Display a message box to the user
        vscode.window.showInformationMessage('Hello World from commentarioxxx!');
        // Get configuration
        let workspaceFolder;
        let workspaceFolderString = "";
        if (vscode.workspace.workspaceFolders !== undefined) {
            workspaceFolder = vscode.workspace.workspaceFolders[0];
            workspaceFolderString = workspaceFolder.uri.path;
        }
        var assemblyPath = vscode.workspace.getConfiguration().get("commentario.assemblyPath");
        if (assemblyPath !== undefined) {
            assemblyPath = assemblyPath.replace('${workspaceFolder}', workspaceFolderString);
        }
        var outputPath = vscode.workspace.getConfiguration().get("commentario.outputPath");
        if (outputPath !== undefined) {
            outputPath = outputPath.replace('${workspaceFolder}', workspaceFolderString);
        }
        var readMePath = vscode.workspace.getConfiguration().get("commentario.readMePath");
        if (readMePath !== undefined) {
            readMePath = readMePath.replace('${workspaceFolder}', workspaceFolderString);
        }
        var xmlCommentsPath = vscode.workspace.getConfiguration().get("commentario.xmlCommentsPath");
        if (xmlCommentsPath !== undefined) {
            xmlCommentsPath = xmlCommentsPath.replace('${workspaceFolder}', workspaceFolderString);
        }
        var outputChannel = vscode.window.createOutputChannel("commentario");
        outputChannel.show();
        outputChannel.appendLine(`commentario.assemblyPath: ${assemblyPath}`);
        outputChannel.appendLine(`commentario.outputPath: ${outputPath}`);
        outputChannel.appendLine(`commentario.xmlCommentsPath: ${xmlCommentsPath}`);
        outputChannel.appendLine(`commentario.readMePath: ${readMePath}`);
        // Call external server
        var extPath = vscode.extensions.getExtension('Dbarone.Commentario').extensionUri.fsPath;
        var consolePath = "/out/server/Dbarone.Net.CommentarioConsole.exe";
        consolePath = `${extPath}${consolePath}`;
        outputChannel.appendLine("");
        outputChannel.appendLine(`extension path: ${consolePath}`);
        const terminal = vscode.window.createTerminal("Open Terminal");
        terminal.show();
        terminal.sendText(`${extPath}\\out\\server\\server.exe Meh`, true); // your command as a string here
        cp.exec(`${extPath}\\out\\server\\server.exe Meh`, (err, stdout, stderr) => {
            vscode.window.showInformationMessage(stdout);
            console.log('stdout: ' + stdout);
            console.log('stderr: ' + stderr);
            if (err) {
                console.log('error: ' + err);
            }
        });
    });
    context.subscriptions.push(disposable);
}
exports.activate = activate;
// This method is called when your extension is deactivated
function deactivate() { }
exports.deactivate = deactivate;
//# sourceMappingURL=extension.js.map