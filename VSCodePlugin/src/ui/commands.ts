import * as vscode from "vscode";
import { LanguageClient } from "vscode-languageclient";
import { DafnyClientProvider } from "../dafnyProvider";
import { DafnyRunner } from "../dafnyRunner";
import { ICompilerResult } from "../serverHelper/ICompilerResult";
import { CommandStrings, Config, EnvironmentConfig, ErrorMsg, InfoMsg, LanguageServerRequest } from "../stringRessources";
import * as path from 'path';

/**
 * VSCode UI Commands
 */
export default class Commands {

    public static showReferences(uri: any, position: any, locations: any) {
        function parsePosition(p: any): vscode.Position {
            return new vscode.Position(p.line, p.character);
        }
        function parseRange(r: any): vscode.Range {
            return new vscode.Range(parsePosition(r.start), parsePosition(r.end));
        }
        function parseLocation(l: any): vscode.Location {
            return new vscode.Location(parseUri(l.uri), parseRange(l.range));
        }
        function parseUri(u: any): vscode.Uri {
            return vscode.Uri.file(u);
        }

        const parsedUri = vscode.Uri.file(uri);
        const parsedPosition = parsePosition(position);
        const parsedLocations = [];
        for (const location of locations) {
            parsedLocations.push(parseLocation(location));
        }

        vscode.commands.executeCommand("editor.action.showReferences", parsedUri, parsedPosition, parsedLocations);
    }

    public extensionContext: vscode.ExtensionContext;
    public languageServer: LanguageClient;
    public provider: DafnyClientProvider;
    public runner: DafnyRunner;

    // tslint:disable: object-literal-sort-keys
    public commands = [
        { name: CommandStrings.ShowReferences, callback: Commands.showReferences, doNotDispose: true },
        { name: CommandStrings.RestartServer, callback: () => this.restartServer() },
        { name: CommandStrings.InstallDafny, callback: () => this.installDafny() },
        { name: CommandStrings.UninstallDafny, callback: () => this.uninstallDafny() },
        /*{
            name: CommandStrings.RequestTest,
            callback: () => {
                vscode.window.showInformationMessage('Aloha');
                console.log("Lets try...");
                this.languageServer.sendRequest(LanguageServerRequest.SayHello).then((answer) => {
                    console.log("Answer: " + answer);
                    vscode.window.showInformationMessage('We did it!');
                }, (e) => {
                    vscode.window.showErrorMessage("Request Test Error: " + e);
                });
            }
        },*/

        { name: CommandStrings.ShowCounterExample, callback: () => {
            if (!vscode.window.activeTextEditor) {
                return;
            }

            vscode.window.activeTextEditor.document.save();
            const arg = { DafnyFile: vscode.window.activeTextEditor.document.fileName}

            this.languageServer.sendRequest(LanguageServerRequest.CounterExample, arg)
            .then((result: any) => {
                ////////////////////////////////////////////////////////////////////////////////////



                const editor: vscode.TextEditor = vscode.window.activeTextEditor!;
                

                let line = result.line - 1;
                let col = result.col;
                if (line < 0) { return null; }

                let variables = "";

                for (let [key, value] of Object.entries(result.variables)) {
                    variables += key + " = " + value + "; ";
                  }

                //for (let j = 0; j < result.variables.length; j++) {
                //    if (j > 0) { variables += ", \n"; }
                //    variables += result.variables[j].Name + "=" + result.variables[j].Value;
                //}


                const renderOptions: vscode.DecorationRenderOptions = {
                    after: {
                        contentText: variables,
                    },
                };

                let decorator: vscode.DecorationOptions = {
                    range: new vscode.Range(new vscode.Position(line, col), new vscode.Position(line, Number.MAX_VALUE)),
                    renderOptions,
                  };
            

            const variableDisplay = vscode.window.createTextEditorDecorationType({
                dark: {
                    after: {
                        backgroundColor: "#cccccc",
                        color: "#161616",
                        margin: "0 0 0 30px",
                    },
                },
                light: {
                    after: {
                        backgroundColor: "#161616",
                        color: "#cccccc",
                    },
                },
            });

            let arrayDannHalt: vscode.DecorationOptions[] = []
            arrayDannHalt.push(decorator)
            if (!vscode.window.activeTextEditor) return null;
            editor.setDecorations(variableDisplay, arrayDannHalt);
        },
    


        
                



                ///////////////////////////////////////////////////////////////////////////////////


        

    
        {
            name: CommandStrings.Compile,
            callback: () => {
                if (!vscode.window.activeTextEditor) {
                    return; // The window was closed before compilation was executed
                }
                return this.compile(vscode.window.activeTextEditor.document);
            },
        },
        {
            name: CommandStrings.CompileAndRun,
            callback: () => {
                if (!vscode.window.activeTextEditor) {
                    return; // The window was closed before compilation was executed
                }
                return this.compile(vscode.window.activeTextEditor.document, true);
            },
        },
        {
            name: CommandStrings.EditText,
            // tslint:disable-next-line:object-literal-sort-keys
            callback: (uri: string, version: number, edits: vscode.TextEdit[]) => this.applyTextEdits(uri, version, edits),
        },
    ];

    constructor(extensionContext: vscode.ExtensionContext, languageServer: LanguageClient, provider: DafnyClientProvider, runner: DafnyRunner) {
        this.languageServer = languageServer;
        this.provider = provider;
        this.runner = runner;
        this.extensionContext = extensionContext;
    }

    /**
     * Register commands listed in @var this.commands to vscode
     */
    public registerCommands() {
        for (const cmd of this.commands) {
            const disposable = vscode.commands.registerCommand(cmd.name, cmd.callback);

            if (cmd.doNotDispose) {
                continue;
            }
            this.extensionContext.subscriptions.push(disposable);
        }
    }

    public restartServer() {
        this.languageServer.sendRequest(LanguageServerRequest.Reset)
            .then(() => true, () => {
                vscode.window.showErrorMessage("Can't restart dafny");
            });
    }

    public installDafny() {
        this.provider.dafnyStatusbar.hideProgress();
        this.provider.dafnyStatusbar.hide();
        this.languageServer.sendRequest(LanguageServerRequest.Install).then((basePath) => {
            console.log("BasePath: " + basePath);
            const config: vscode.WorkspaceConfiguration = vscode.workspace.getConfiguration(EnvironmentConfig.Dafny);
            config.update(Config.DafnyBasePath, basePath, true).then(() => {
                vscode.window.showInformationMessage("Installation finished");
                this.provider.dafnyStatusbar.hideProgress();
            });
        }, (e) => {
            vscode.window.showErrorMessage("Installing error: " + e);
            this.provider.dafnyStatusbar.hideProgress();
        });
    }

    public uninstallDafny() {
        this.languageServer.sendRequest(LanguageServerRequest.Uninstall).then(() => {
            vscode.window.showInformationMessage("Uninstall complete");
            this.provider.dafnyStatusbar.hideProgress();
            this.provider.dafnyStatusbar.hide();
        }, (e) => {
            vscode.window.showErrorMessage("Can't uninstall dafny:" + e);
            this.provider.dafnyStatusbar.hideProgress();
            this.provider.dafnyStatusbar.hide();
        });
    }

    public compile(document: vscode.TextDocument | undefined, run: boolean = false): void {
        if (!document) {
            return; // Skip if user closed everything in the meantime
        }
        document.save();
        vscode.window.showInformationMessage(InfoMsg.CompilationStarted);

        const dafnyExe = path.join(__dirname, "../../../../dafny/Binaries/Dafny.exe")   //TODO: Production Folder Structure may be different. Sollte man auch auslagern.
        const arg = {
            DafnyFilePath: document.fileName,
            DafnyExePath: dafnyExe
        }

        this.languageServer.sendRequest<ICompilerResult>(LanguageServerRequest.Compile, arg)
        .then((result) => {
            if (result.error) {
                vscode.window.showErrorMessage(result.message || InfoMsg.CompilationFailed);
                return true;
            }
            vscode.window.showInformationMessage(result.message || InfoMsg.CompilationFinished)
            if (run) {
                if (result.executable) {
                    vscode.window.showInformationMessage(InfoMsg.CompilationStartRunner);
                    this.runner.run(document.fileName);
                } else {
                    vscode.window.showInformationMessage(ErrorMsg.NoMainMethod);
                }
            }
            return true;
        }, (error: any) => {
            vscode.window.showErrorMessage("Can't compile: " + error.message);
        });
    }

    public applyTextEdits(uri: string, documentVersion: number, edits: vscode.TextEdit[]) {
        const textEditor = vscode.window.activeTextEditor;

        if (textEditor && textEditor.document.uri.toString() === uri) {
            if (textEditor.document.version !== documentVersion) {
                console.log("Versions of doc are different");
            }
            textEditor.edit((mutator: vscode.TextEditorEdit) => {
                for (const edit of edits) {
                    mutator.replace(this.languageServer.protocol2CodeConverter.asRange(edit.range), edit.newText);
                }
            }).then((success) => {
                if (!success) {
                    vscode.window.showErrorMessage("Failed to apply changes to the document.");
                }
            });
        }
    }
}
