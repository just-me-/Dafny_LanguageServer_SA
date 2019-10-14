import { ExtensionContext, workspace, env } from "vscode";
import { LanguageClient, ServerOptions } from "vscode-languageclient";
import { TransportKind, LanguageClientOptions, VersionedTextDocumentIdentifier } from "vscode-languageclient/lib/client";
import { window } from 'vscode';

export default class DafnyLanguageClient extends LanguageClient {

    constructor(extensionContext: ExtensionContext) {
        
        // The server is implemented in node
        const serverExe = 'dotnet';
        // relativer pfad funzt ned. Evt über eine Config var angeben dass es etwas eleganter ist?
        // unbedingt ;-) \t
        const path_marcel = '/Users/marcel/Documents/HSR/5. Semester/SA/_Code/dafny-server-redesign/Dafny_Server_Redesign/Dafny_Server_Redesign/bin/Debug/netcoreapp2.1/Dafny_Server_Redesign.dll';
        const path_tom_laptop = 'D:\\Eigene Dokumente\\VisualStudio\\SA\\dafny-server-redesign\\dafny\\Binaries\\DafnyLanguageServer.exe'
        const path_tom_desktop = 'G:\\Dokumente\\VisualStudio\\SA\\dafny-server-redesign\\dafny\\Binaries\\DafnyLanguageServer.exe'
        const path_marcel_win = 'C:\\Users\\Marcel\\Desktop\\SA\\dafny-server-redesign\\Dafny_Server_Redesign\\Dafny_Server_Redesign\\bin\\Debug\\netcoreapp2.1\\Dafny_Server_Redesign.dll'
        const path = 
        env.appRoot.match('marcel') !== null ? path_marcel : (
        env.appRoot.match('Marcel') ? path_marcel_win : (
        env.appRoot == 'c:\\ProgramData\\Microsoft VS Code\\resources\\app' ? path_tom_desktop : path_tom_laptop
        )
        );

        console.log(env.appRoot);
        

        window.showInformationMessage("Chosen Path: " + path);
        
        // If the extension is launched in debug mode then the debug server options are used
        // Otherwise the run options are used
        const serverOptions: ServerOptions = {
            run: { command: path, args: [] },
            debug: { command: path, args: [] }
            /* old node config: 
            debug: {
                module: serverModule,
                options: {
                    execArgv: ["--nolazy", "--inspect=6009"],
                },
                transport: TransportKind.ipc,
            },
            run: {
                module: serverModule,
                transport: TransportKind.ipc,
            },
            */
        }
    
        // Options to control the language client
        const clientOptions: LanguageClientOptions = {
            // Register the server for plain text documents
            documentSelector: [
                {
                    pattern: '**/*.dfy', // aus Tutorial
                },
                {
                    language: "dafny",
                    scheme: "file",
                }
            ],
            synchronize: {
                /* aus tutorial */
                // Synchronize the setting section 'languageServerExample' to the server
                //configurationSection: 'languageServerExample',
                fileEvents: workspace.createFileSystemWatcher('**/*.dfy'),
                // aus node
                configurationSection: "dafny",
            },
        }

        super("dafny-vscode", "Dafny Language Server", serverOptions, clientOptions);
    }
}
