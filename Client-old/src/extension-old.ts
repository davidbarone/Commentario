// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
const cp = require('child_process');


// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
	// Use the console to output diagnostic information (console.log) and errors (console.error)
	// This line of code will only be executed once when your extension is activated
	console.log('Congratulations, your extension "helloworld-sample" is now active!');

	// The command has been defined in the package.json file
	// Now provide the implementation of the command with registerCommand
	// The commandId parameter must match the command field in package.json
	let disposable = vscode.commands.registerCommand('commentario.helloWorld', () => {
		// The code you place here will be executed every time your command is executed

		// Display a message box to the user
		//vscode.window.showInformationMessage('Hello World!');
		var filePath = vscode.workspace.getConfiguration().get("assemblyPath");
		
		// Call external server
		var extPath: string = vscode.extensions.getExtension('dbarone.commentario')!.extensionUri.fsPath;

		const terminal = vscode.window.createTerminal("Open Terminal");
		terminal.show();
		terminal.sendText(`${extPath}\\out\\server\\server.exe Meh`, true);   // your command as a string here

		cp.exec(`${extPath}\\out\\server\\server.exe Meh`, (err: any, stdout: any, stderr: any) => {
			vscode.window.showInformationMessage(stdout);
			console.log('stdout: ' + stdout);
			console.log('stderr: ' + stderr);
			if (err) {
				console.log('error: ' + err);
			}
		});		
		var a = 123;
	});

	context.subscriptions.push(disposable);
}

// this method is called when your extension is deactivated
export function deactivate() { }