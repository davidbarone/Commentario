// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
const cp = require('child_process');

// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {

	// Use the console to output diagnostic information (console.log) and errors (console.error)
	// This line of code will only be executed once when your extension is activated
	console.log('Commentario activated.');

	// The command has been defined in the package.json file
	// Now provide the implementation of the command with registerCommand
	// The commandId parameter must match the command field in package.json
	let disposable = vscode.commands.registerCommand('commentario.createDocumentation', () => {
		// The code you place here will be executed every time your command is executed
		// Display a message box to the user
		vscode.window.showInformationMessage('Starting Commentario...');

		// Get configuration
		let workspaceFolder: vscode.WorkspaceFolder;
		let workspaceFolderString: string = "";
		if (vscode.workspace.workspaceFolders !== undefined) {
			workspaceFolder = vscode.workspace.workspaceFolders[0];
			workspaceFolderString = workspaceFolder.uri.fsPath;
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
		if (readMePath !== undefined && readMePath.trim() !== "") {
			readMePath = readMePath.replace('${workspaceFolder}', workspaceFolderString);
		}

		if (readMePath !== undefined && readMePath.trim() === "") {
			readMePath = undefined;
		}

		var stylesPath: string | undefined = vscode.workspace.getConfiguration().get("commentario.stylesPath");
		if (stylesPath !== undefined && stylesPath.trim() !== "") {
			stylesPath = stylesPath.replace('${workspaceFolder}', workspaceFolderString);
		}

		if (stylesPath !== undefined && stylesPath.trim() === "") {
			stylesPath = undefined;
		}

		var xmlCommentsPath: string | undefined = vscode.workspace.getConfiguration().get("commentario.xmlCommentsPath");
		if (xmlCommentsPath !== undefined && xmlCommentsPath.trim() !== "") {
			xmlCommentsPath = xmlCommentsPath.replace('${workspaceFolder}', workspaceFolderString);
		}

		if (xmlCommentsPath !== undefined && xmlCommentsPath.trim() === "") {
			xmlCommentsPath = undefined;
		}

		var outputType: string | undefined = vscode.workspace.getConfiguration().get("commentario.outputType");
		var debugMode: string | undefined = vscode.workspace.getConfiguration().get("commentario.debugMode");

		var outputChannel = vscode.window.createOutputChannel("Commentario");
		outputChannel.show();
		outputChannel.appendLine("Starting commentario.createDocumentation...");
		outputChannel.appendLine("");
		outputChannel.appendLine("Configuration");
		outputChannel.appendLine("-------------");
		outputChannel.appendLine(`WorkspaceFolder: ${workspaceFolderString}`);
		outputChannel.appendLine(`commentario.assemblyPath: ${assemblyPath}`);
		outputChannel.appendLine(`commentario.outputPath: ${outputPath}`);
		outputChannel.appendLine(`commentario.allowOverwrite: ${allowOverwrite}`);
		outputChannel.appendLine(`commentario.xmlCommentsPath: ${xmlCommentsPath}`);
		outputChannel.appendLine(`commentario.readMePath: ${readMePath}`);
		outputChannel.appendLine(`commentario.stylesPath: ${stylesPath}`);
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
			cmd = `${cmd} -c ${xmlCommentsPath}`;
		}

		if (readMePath !== undefined) {
			cmd = `${cmd} -r ${readMePath}`;
		}

		if (stylesPath !== undefined) {
			cmd = `${cmd} -s ${stylesPath}`;
		}

		if (outputType !== undefined) {
			cmd = `${cmd} -t ${outputType}`;
		}

		if (allowOverwrite === "true") {
			cmd = `${cmd} -o`;
		}

		if (debugMode === "true") {
			cmd = `${cmd} -d`;
		}

		outputChannel.appendLine("");
		outputChannel.appendLine(`Executing command: ${cmd}`);

		//const terminal = vscode.window.createTerminal("Commentario");
		//terminal.show();
		//terminal.sendText("Generating documentation using Commentario...\n", false);
		cp.exec(`${cmd}`, (err: any, stdout: any, stderr: any) => {
			outputChannel.appendLine(stdout);
			outputChannel.appendLine(stderr);
			outputChannel.appendLine("Completed commentario.createDocumentation...");
		});
	});

	context.subscriptions.push(disposable);
}

// This method is called when your extension is deactivated
export function deactivate() { }
