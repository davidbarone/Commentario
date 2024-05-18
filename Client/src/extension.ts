// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
const cp = require('child_process');

// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {

	// Use the console to output diagnostic information (console.log) and errors (console.error)
	// This line of code will only be executed once when your extension is activated
	console.log('Congratulations, your extension "commentario" is now active!');

	// The command has been defined in the package.json file
	// Now provide the implementation of the command with registerCommand
	// The commandId parameter must match the command field in package.json
	let disposable = vscode.commands.registerCommand('commentario.createDocumentation', () => {
		// The code you place here will be executed every time your command is executed
		// Display a message box to the user
		vscode.window.showInformationMessage('dbarone.commentario is running.');

		// Get configuration
		let workspaceFolder: vscode.WorkspaceFolder;
		let workspaceFolderString: string = "";
		if (vscode.workspace.workspaceFolders !== undefined) {
			workspaceFolder = vscode.workspace.workspaceFolders[0];
			workspaceFolderString = workspaceFolder.uri.path;
		}

		var assemblyPath: string | undefined = vscode.workspace.getConfiguration().get("commentario.assemblyPath");
		if (assemblyPath !== undefined) {
			assemblyPath = assemblyPath.replace('${workspaceFolder}', workspaceFolderString);
		}

		var outputPath: string | undefined = vscode.workspace.getConfiguration().get("commentario.outputPath");
		if (outputPath !== undefined) {
			outputPath = outputPath.replace('${workspaceFolder}', workspaceFolderString);
		}

		var allowOverwrite: string | undefined = vscode.workspace.getConfiguration().get("commentario.allowOverwrite");

		var readMePath: string | undefined = vscode.workspace.getConfiguration().get("commentario.readMePath");
		if (readMePath !== undefined) {
			readMePath = readMePath.replace('${workspaceFolder}', workspaceFolderString);
		}

		var xmlCommentsPath: string | undefined = vscode.workspace.getConfiguration().get("commentario.xmlCommentsPath");
		if (xmlCommentsPath !== undefined) {
			xmlCommentsPath = xmlCommentsPath.replace('${workspaceFolder}', workspaceFolderString);
		}

		var outputType: string | undefined = vscode.workspace.getConfiguration().get("commentario.outputType");
		var debugMode: string | undefined = vscode.workspace.getConfiguration().get("commentario.debugMode");

		var outputChannel = vscode.window.createOutputChannel("commentario");
		outputChannel.show();
		outputChannel.appendLine("Commentario Client Debugging");
		outputChannel.appendLine("----------------------------");
		outputChannel.appendLine(`commentario.assemblyPath: ${assemblyPath}`);
		outputChannel.appendLine(`commentario.outputPath: ${outputPath}`);
		outputChannel.appendLine(`commentario.allowOverwrite: ${allowOverwrite}`);
		outputChannel.appendLine(`commentario.xmlCommentsPath: ${xmlCommentsPath}`);
		outputChannel.appendLine(`commentario.readMePath: ${readMePath}`);
		outputChannel.appendLine(`commentario.outputType: ${outputType}`);
		outputChannel.appendLine(`commentario.debugMode: ${debugMode}`);

		// Call external server
		var extPath: string = vscode.extensions.getExtension('dbarone.commentario')!.extensionUri.fsPath;
		var consolePath: string = "/out/server/Dbarone.Net.CommentarioConsole.exe";
		consolePath = `${extPath}${consolePath}`;
		outputChannel.appendLine("");
		outputChannel.appendLine(`extension path: ${consolePath}`);

		// Calculate the command
		var cmd: string = `${consolePath} ${assemblyPath} ${outputPath}`;
		if (xmlCommentsPath !== undefined) {
			cmd = `${cmd} -x ${xmlCommentsPath}`;
		}

		if (outputType !== undefined) {
			cmd = `${cmd} -t ${outputType}`;
		}

		if (allowOverwrite !== undefined) {
			cmd = `${cmd} -o ${allowOverwrite}`;
		}

		outputChannel.appendLine("");
		outputChannel.appendLine(`Executing command: ${cmd}`);

		//const terminal = vscode.window.createTerminal("Open Terminal");
		//terminal.show();
		//terminal.sendText("Command:\n", false);
		//terminal.sendText("--------\n", false);
		//terminal.sendText(`${cmd}\n`, false);
		cp.exec(`${cmd}`, (err: any, stdout: any, stderr: any) => {
			outputChannel.appendLine(stdout);

			if (err) {
				outputChannel.appendLine(stderr);
			}
		});
	});

	context.subscriptions.push(disposable);
}

// This method is called when your extension is deactivated
export function deactivate() { }
