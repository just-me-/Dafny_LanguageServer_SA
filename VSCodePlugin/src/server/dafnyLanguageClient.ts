import { ExtensionContext, workspace } from "vscode";
import { LanguageClient, ServerOptions } from "vscode-languageclient";
import { TransportKind, LanguageClientOptions } from "vscode-languageclient/lib/client";

export default class DafnyLanguageClient extends LanguageClient {

    constructor(extensionContext: ExtensionContext) {
        
        // The server is implemented in node
        const serverExe = 'dotnet';
        // relativer pfad funzt ned. Evt über eine Config var angeben dass es etwas eleganter ist?
        // unbedingt ;-) \t
        const path_marcel = '/Users/marcel/Documents/HSR/5. Semester/SA/_Code/dafny-server-redesign/Dafny_Server_Redesign/Dafny_Server_Redesign/bin/Debug/netcoreapp2.1/Dafny_Server_Redesign.dll';
        const path_tom_laptop = 'D:\\Eigene Dokumente\\VisualStudio\\SA\\dafny-server-redesign\\Dafny_Server_Redesign\\Dafny_Server_Redesign\\bin\\Debug\\netcoreapp2.1\\Dafny_Server_Redesign.dll'
        
        const path = path_tom_laptop
        // If the extension is launched in debug mode then the debug server options are used
        // Otherwise the run options are used
        const serverOptions: ServerOptions = {
            run: { command: serverExe, args: [path] },
            debug: { command: serverExe, args: [path] }
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
