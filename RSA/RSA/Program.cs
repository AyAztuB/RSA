using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            /*List<string> OPTION = new List<string>(){"--REPL", "--GenerateKeys", "--Encrypt", "--Decrypt"};
            if (args.Length == 0 || !OPTION.Contains(args[0]))
                DisplayHelp();
            else
                DisplayHelp(args[0][2..]);*/
            if (args.Length != 0 && args[0] == "--GenerateKeys")
                GenerateKeysReadOption(args);
            else
                DisplayHelp();
        }


        #region Option

        #region HelpMessages

        public static void DisplayHelp(string option = "")
        {
            Console.WriteLine("This program can create RSA keys, decrypt and encrypt messages using the RSA crypto-system and can break some very small RSA keys (only in source code) !\n");
            if (option == "")
            {
                System.Console.WriteLine("run <OPTION> [--h] [--c pathToCommonKeyPart] [--k keyPath] [--m message] [--f File] [--x] [--o filename] [--j useJson] [--r nbRound] [--l RSAlength]");
                System.Console.WriteLine();
                System.Console.WriteLine("Available OPTIONs :");
                System.Console.WriteLine();
                System.Console.WriteLine("--REPL        \t=> Currently in development !\n");
                System.Console.WriteLine("--GenerateKeys\t=> Generate public and private key to write them in a [json] file\n");
                System.Console.WriteLine("--Encrypt     \t=> Use key file to encrypt a message/file/directory\n");
                System.Console.WriteLine("--Decrypt     \t=> Use key file to decrypt a message/file/directory\n");
            }
            else if (option == "REPL")
                System.Console.WriteLine("Currently in development\nNot yet available !\nThanks for your understanding !\n");
            else if (option == "GenerateKeys")
            {
                System.Console.WriteLine("run --GenerateKeys [--h] [--c path_to_common_key_part] [--k keyPath] [--j jsonFile] [--r nbRound] [--l RSAlength]\n");
                System.Console.WriteLine("Available arguments :\n");
                System.Console.WriteLine("--h \t=> display this help ^^\n");
                System.Console.WriteLine("--j \t=> transform RSA object in json file\n   \t if the file exists, it will be overridden, otherwise, it will be created !\n");
                System.Console.WriteLine("--c \t=> the file where the encoded common part of the keys will be written\n   \t if the file exists, it will be overridden, otherwise, it will be created !\n");
                System.Console.WriteLine("--k \t=> the file where the private encoded key will be written. The public key will be written in the same location with .pub extension");
                System.Console.WriteLine("    \t if the file exists, it will be overridden, otherwise, it will be created !\n");
                System.Console.WriteLine("--r \t=> number of rounds for the prime number generation [default : 30]\n");
                System.Console.WriteLine("--l \t=> length of the prime numbers generated [default : 256 bytes ~ 616 digits]\n");
                System.Console.WriteLine("OTHERS ARGUMENTS ARE UNAVAILABLE !!!!\n");
                System.Console.WriteLine("--c and --k cannot be used without the other one, you can use [--c <name> --k <name>] and [--j <name>] to save the key multiple time.");
                System.Console.WriteLine("If you don't use [--c <name> --k <name>] neither [--j <name>], all the keys will be displayed in the encoded format in the console.");
                System.Console.WriteLine("If an argument is incorrect or if the path given is invalid, this help page will be displayed.\n");
                System.Console.WriteLine("The argument order doesn't have any importance.\n");
            }
            else
            {
                System.Console.WriteLine($"run --{option} [--h] [--c path_to_common_key_part] [--k keyPath] [--j jsonFile] [--m message] [--f file] [--x] [--o outputName]\n");
                System.Console.WriteLine("Available arguments :\n");
                System.Console.WriteLine("--h \t=> display this help ^^\n");
                System.Console.WriteLine("--c \t=> the file where the encoded common part of the key has been written\n");
                System.Console.WriteLine($"--k \t=> the file where the {(option == "Encrypt" ? "public" : "private")} key has been written\n");
                System.Console.WriteLine("--j \t=> the json file where the RSA object has been saved\n");
                System.Console.WriteLine("--m \t=> message to " + option.ToLower() + '\n');
                System.Console.WriteLine("--f \t=> the file (or directory with --x necessarily) to " + option.ToLower() + '\n');
                System.Console.WriteLine("--x \t=> used to encrypt all files in a given directory. Cannot be applied without --f");
                if (option == "Encrypt")
                    System.Console.WriteLine("   \t    if it's applied to a large file, it forces the encryption, splitting the file content and encrypting each part. The output will be a directory (with the given name) where each part will be encrypted in a file <name>(number) [without extension]");
                System.Console.WriteLine("\n--o \t=> precise an output file or directory. If it's a directory, each " + option.ToLower()+"ed file will be added or overridden in the output directory with the same name as the original file.\n");
                System.Console.WriteLine("OTHERS ARGUMENTS ARE UNAVAILABLE !!!!\n");
                System.Console.WriteLine("--c and --k cannot be used without the other one, you cannot use [--c <name> --k <name>] and [--j <name>] at the same time.");
                System.Console.WriteLine("One of the two options ([--j] or [--c --k]) is necessary !");
                System.Console.WriteLine("If [--o] isn't precise, if [--m] is used, then the output will be displayed in the terminal, else the output will override the given file !");
                System.Console.WriteLine("--f and --m cannot be used at the same time.");
                System.Console.WriteLine("If an argument is incorrect or if the path/file given is invalid, this help page will be displayed.\n");
                System.Console.WriteLine("The argument order doesn't have any importance.\n");
            }
            System.Console.WriteLine();
        }

        #endregion

        #region REPL
        // TODO
        #endregion

        #region GenerateKeys

        private static void GenerateKeysReadOption(string[] args)
        {
            string help = "GenerateKeys";
            string json = "";
            string common = "";
            string key = "";
            int rounds = 30;
            int length = 256;
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--j":
                        if (json != "" || i+1 >= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i+= 1;
                        json = args[i];
                        break;
                    case "--c":
                        if (common != "" || i+1 >= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i+=1;
                        common = args[i];
                        break;
                    case "--k":
                        if (key != "" || i+1 >= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i+=1;
                        key = args[i];
                        break;
                    case "--r":
                        if (i+1 >= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i+=1;
                        try{
                            rounds = Int32.Parse(args[i]);
                        }
                        catch (Exception){
                            Console.ForegroundColor = ConsoleColor.Red;
                            System.Console.WriteLine("FAIL: ROUND VALUE MUST BE A NUMBER !");
                            Console.ForegroundColor = ConsoleColor.White;
                            DisplayHelp(help);
                            return;
                        }
                        if (rounds <= 0){
                            Console.ForegroundColor = ConsoleColor.Red;
                            System.Console.WriteLine("FAIL: ROUND VALUE MUST BE SUPERIOR THAN 0 !");
                            Console.ForegroundColor = ConsoleColor.White;
                            DisplayHelp(help);
                            return;
                        }
                        break;
                    case "--l":
                        if (i+1 >= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i+=1;
                        try{
                            length = Int32.Parse(args[i]);
                        }
                        catch (Exception){
                            Console.ForegroundColor = ConsoleColor.Red;
                            System.Console.WriteLine("FAIL: LENGTH VALUE MUST BE A NUMBER !");
                            Console.ForegroundColor = ConsoleColor.White;
                            DisplayHelp(help);
                            return;
                        }
                        if (length <= 0){
                            Console.ForegroundColor = ConsoleColor.Red;
                            System.Console.WriteLine("FAIL: LENGTH VALUE MUST BE SUPERIOR THAN 0 !");
                            Console.ForegroundColor = ConsoleColor.White;
                            DisplayHelp(help);
                            return;
                        }
                        break;
                    default:
                        DisplayHelp(help);
                        return;
                }
            }
            if ((common == "" && key != "") || (key == "" && common != "")){
                DisplayHelp(help);
                return;
            }
            GenerateKeysWithOption(json, common, key, rounds, length);
        }

        private static void GenerateKeysWithOption(string json, string common, string key, int rounds, int length)
        {
            string help = "GenerateKeys";
            // VERIFIE PATH TO JSON, COMMON, KEY
            if (json != "" && !IsPathCorrect(json)){
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("FAIL: THE GIVEN JSON FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                System.Console.WriteLine("complete path: " + GetCompletePath(json));
                Console.ForegroundColor = ConsoleColor.White;
                DisplayHelp(help);
                return;
            }
            if (common != "" && !IsPathCorrect(common)){
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("FAIL: THE GIVEN COMMON FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                System.Console.WriteLine("complete path: " + GetCompletePath(common));
                Console.ForegroundColor = ConsoleColor.White;
                DisplayHelp(help);
                return;
            }
            if (key != "" && !IsPathCorrect(key)){
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("FAIL: THE GIVEN KEY FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                System.Console.WriteLine("complete path: " + GetCompletePath(key));
                Console.ForegroundColor = ConsoleColor.White;
                DisplayHelp(help);
                return;
            }
            if (common != "" && (common[^1] == '/' || common[^1] == '\\'))
                common += "common";
            if (key != ""){
                if (key[^1] == '/' || key[^1] == '\\')
                    key += "rsa";
                else if(key.Split(new char[]{'/', '\\'})[^1].Contains('.')){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN KEY FILE HAVE AN EXTENSION !");
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
            }
            if (json != ""){
                if (json[^1] == '/' || json[^1] == '\\')
                    json += "RSA.json";
                else {
                    string last = json.Split(new char[]{'/','\\'})[^1];
                    if (last.Length < 6 || last[^5..] != ".json"){
                        Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("FAIL: THE GIVEN JSON FILE HAVE A BAD EXTENSION !");
                        Console.ForegroundColor = ConsoleColor.White;
                        DisplayHelp(help);
                        return;
                    }
                }
            }
            RSA rsa = new RSA(length, rounds);
            if (json != ""){
                if (WriteRSAFile(rsa, json)){
                    Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("Successfully create .json rsa file");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: IMPOSSIBLE TO WRITE RSA IN JSON FILE !");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            if (key != "" && common != ""){
                if (WriteFile(common, rsa.commonKeyParameter, true)){
                    Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("Successfully write common key parameter");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: IMPOSSIBLE TO WRITE RSA COMMON KEY PARAMETER !");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (WriteFile(key, rsa.privKey, true)){
                    Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("Successfully write private key parameter");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: IMPOSSIBLE TO WRITE RSA PRIVATE KEY PARAMETER !");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (WriteFile(key+".pub", rsa.pubKey, true)){
                    Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("Successfully write public key parameter");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: IMPOSSIBLE TO WRITE RSA PUBLIC KEY PARAMETER !");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else if (json == ""){
                Console.WriteLine("COMMON RSA KEY:");
                System.Console.WriteLine(rsa.commonKeyParameter + '\n');
                System.Console.WriteLine("PUBLIC KEY:\n"+rsa.pubKey+'\n');
                System.Console.WriteLine("PRIVATE KEY:\n"+rsa.privKey+'\n');
            }
        }

        #endregion

        #region Encrypt-Decrypt

        private static void ReadOptionEncryptDecrypt(bool isEncrypt, string[] args){
            string help = isEncrypt ? "Encrypt" : "Decrypt";
            string json = "", common = "", key = "";
            string message = "", file = "";
            bool force = false;
            string output = "";

            for (int i = 1; i < args.Length; i++){
                switch(args[i]){
                    case "--j" :
                        if (json != "" || common != "" || key != "" || i+1 >= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i+=1;
                        json = args[i];
                        break;
                    case "--k" :
                    case "--c" :
                        if (json != "" || (args[i] == "--c" && common != "") || (args[i] == "--k" && key != "") || i+1 >= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i +=1;
                        if (args[i-1] == "--c")
                            common = args[i];
                        else
                            key = args[i];
                        break;
                    case "--x" :
                        if (force){
                            DisplayHelp(help);
                            return;
                        }
                        force = true;
                        break;
                    case "--f" :
                    case "--m" :
                        if (message != "" || file != "" || i+1>= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i+=1;
                        if (args[i-1] == "--f")
                            file = args[i];
                        else
                            message = args[i];
                        break;
                    case "--o" :
                        if (output != "" || i+1 >= args.Length || args[i+1].Length == 0){
                            DisplayHelp(help);
                            return;
                        }
                        i+=1;
                        output = args[i];
                        break;
                    default:
                        DisplayHelp(help);
                        return;
                }
            }
            if ((common != "" && key == "") || (key != "" && common == "") || (json == "" && common == "") || (json != "" && common != "") || (message == "" && file == "")){
                DisplayHelp(help);
                return;
            }
            if (force && message != ""){
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("FAIL: INVALID -x ARGUMENT WITH -m !");
                Console.ForegroundColor = ConsoleColor.White;
                DisplayHelp(help);
                return;
            }
            if (force && !isEncrypt && (file[^1] != '/' && file[^1] != '\\')){
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("FAIL: INVALID -x ARGUMENT WITH --Decrypt OPTION IF IT'S NOT A DIRECTORY !");
                Console.ForegroundColor = ConsoleColor.White;
                DisplayHelp(help);
                return;
            }
            EncryptDecryptWithOption(isEncrypt, json == "" ? key : json, common, message == "" ? file : message, message == "", output, force);
        }

        private static void EncryptDecryptWithOption(bool isEncrypt, string key, string common, string message, bool isFile, string output, bool force){
            string help = isEncrypt ? "Encrypt" : "Decrypt";
            string COMMON = "", KEY = "";
            string MESSAGE = isFile ? "" : message;
            List<(string, string)> DIRECTORY = null;
            
            if (common == ""){
                if (key[^1] == '/' || key[^1] == '\\')
                    key += "RSA.json";
                if (! IsPathCorrect(key)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN JSON FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                    System.Console.WriteLine("complete path: " + GetCompletePath(key));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                (bool isOk, RSA rsa) = ReadRSAFile(key);
                if (!isOk){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN JSON FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH / FILE");
                    System.Console.WriteLine("complete path: " + GetCompletePath(key));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                COMMON = rsa.commonKeyParameter;
                KEY = isEncrypt ? rsa.pubKey : rsa.privKey;
            }
            else{
                if (key[^1] == '/' || key[^1] == '\\')
                    key += ("rsa" + (isEncrypt ? ".pub" : ""));
                if (common[^1]=='/' || common[^1]=='\\')
                    common += "common";
                if (!IsPathCorrect(key)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN KEY FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                    System.Console.WriteLine("complete path: " + GetCompletePath(key));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                if (!IsPathCorrect(common)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN COMMON FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                    System.Console.WriteLine("complete path: " + GetCompletePath(common));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                if (!ReadFile(key, out KEY)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN KEY FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH / FILE");
                    System.Console.WriteLine("complete path: " + GetCompletePath(key));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                if (!ReadFile(common, out COMMON)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN COMMON FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH / FILE");
                    System.Console.WriteLine("complete path: " + GetCompletePath(common));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
            }

            if (output!= "" && ! IsPathCorrect(output)){
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("FAIL: THE GIVEN OUTPUT CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                System.Console.WriteLine("complete path: " + GetCompletePath(output));
                Console.ForegroundColor = ConsoleColor.White;
                DisplayHelp(help);
                return;
            }

            if (isFile && (message[^1] == '/' || message[^1] == '\\') && !force){
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"FAIL: PLEASE ADD -x ARGUMENT TO {(isEncrypt ? "ENCRYPT" : "DECRYPT")} A DIRECTORY !");
                Console.ForegroundColor = ConsoleColor.White;
                DisplayHelp(help);
                return;
            }
            else if (isFile && message[^1] != '/' && message[^1] != '\\'){
                if (!IsPathCorrect(message)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN MESSAGE FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                    System.Console.WriteLine("complete path: " + GetCompletePath(message));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                if (!ReadFile(message, out MESSAGE)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN MESSAGE FILE CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                    System.Console.WriteLine("complete path: " + GetCompletePath(message));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
            }
            else if (isFile){
                if (!IsPathCorrect(message)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN DIRECTORY CANNOT BE ACCESSED DU TO THE INCORRECT PATH");
                    System.Console.WriteLine("complete path: " + GetCompletePath(message));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                DIRECTORY = ReadAllDirectory(message);
            }

            if (!force || (message[^1] != '\\' && message[^1] != '/' && RSA.FromBinary(MESSAGE) < RSA.FromBinary(COMMON))){
                string RES = "";
                if (isEncrypt){
                    var res = RSA.EncryptMessage(MESSAGE, COMMON, KEY);
                    if (!res.Item2){
                        Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("FAIL: THE MESSAGE IS TOO LONG FOR THE COMMON KEY !");
                        Console.ForegroundColor = ConsoleColor.White;
                        DisplayHelp(help);
                        return;
                    }
                    RES = res.Item1;
                }
                else{
                    RES = RSA.DecryptMessage(MESSAGE, COMMON, KEY);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("Successfully " + (isEncrypt ? "encrypt" : "decrypt") + " the message" + (isFile ? " in the file" : ""));
                Console.ForegroundColor = ConsoleColor.White;
                if (output == ""){
                    if (isFile){
                        if (!WriteFile(message, RES, false)){
                            Console.ForegroundColor = ConsoleColor.Red;
                            System.Console.WriteLine("FAIL: IMPOSSIBLE TO WRITE IN THE FILE !");
                            System.Console.WriteLine("complete path: " + GetCompletePath(message));
                            Console.ForegroundColor = ConsoleColor.White;
                            DisplayHelp(help);
                            return;
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        System.Console.WriteLine("Successfully write message in the file");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    System.Console.WriteLine((isEncrypt ? "ENCRYPTED" : "DECRYPTED") + " MESSAGE :\n" + RES);
                    return;
                }
                if (output[^1] == '/'|| output[^1] == '\\')
                    output += (isFile ? message.Split(new char[] {'/','\\'})[^1] : "output.out");
                if(! WriteFile(output, RES, true)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: IMPOSSIBLE TO WRITE IN THE OUTPUT FILE !");
                    System.Console.WriteLine("complete path: " + GetCompletePath(output));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("Successfully write " + (isEncrypt ? "encrypted" : "decrypted") + " message in '" + GetCompletePath(output) + "'");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            if (message[^1] != '\\' && message[^1] != '/'){
                EncrypLongFiles(MESSAGE, output, COMMON, KEY);
                return;
            }
            // TODO ENCRYPT/DECRYPT DIRECTORY
        }

        private static void EncrypLongFiles(string message, string output, string common, string key){
            string help = "Encrypt";
            List<string> messages = new List<string>();
            string m = "";
            for (int i = 0; i < message.Length; i++){
                if (m.Length >= common.Length -1){
                    messages.Add(m);
                    m = "";
                }
                m += message[i];
            }
            List<string> encrypted = new List<string>();
            foreach(var s in messages){
                (string r, bool ok) = RSA.EncryptMessage(s, common, key);
                if (!ok){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: AN ERROR APPAIRS DURING THE ENCRYPTION !");
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
                encrypted.Add(r);
            }
            if (output == "")
                output = "output/";
            Directory.CreateDirectory(output);
            for(int i = 0; i < encrypted.Count; i++){
                if (!WriteFile(output + $"output({i+1}).out", encrypted[i], true)){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: IMPOSSIBILITY TO WRITE IN THE OUTPUT DIRECTORY FILES");
                    System.Console.WriteLine("complete path: " + GetCompletePath(output + $"output({i+1}.out)"));
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Successfully write all file part encrypted in the output directory");
            Console.ForegroundColor = ConsoleColor.White;
        }

        #endregion

        #endregion


        private static bool IsPathCorrect(string path){
            if (path[^1] == '/' || path[^1] == '\\')
                return Directory.Exists(path);
            return Directory.Exists(GetFileDir(path));
        }

        private static string GetFileDir(string path){
            string[] split = path.Split(new char[]{'/', '\\'});
            string fileDir = "";
            for (int i = 0; i < split.Length -1; i++){
                fileDir += (split[i] + "/");
            }
            return fileDir;
        }

        private static string GetCompletePath(string path){
            if (path[0] == '/'|| path[0] == '\\')
                return path;
            string[] execDir = (new DirectoryInfo("./")).FullName[..^1].Split(new char[]{'/', '\\'});
            string[] pathSplit = path.Split(new char[]{'/','\\'});
            List<string> resSplit = new List<string>();
            foreach(var str in execDir){
                resSplit.Add(str);
            }
            foreach(var k in pathSplit){
                if (k == "..")
                    resSplit.RemoveAt(resSplit.Count - 1);
                else if (k != ".")
                    resSplit.Add(k);
            }
            string res = "";
            foreach(var i in resSplit){
                res += ("/" + i);
            }
            return res;
        }


        #region RSA_from_json

        public static (bool, RSA) ReadRSAFile(string path = "RSA.json")
        {
            string rsaJson = "";
            if (ReadFile(path, out rsaJson)){
                RSA rsa;
                try{
                    rsa = JsonSerializer.Deserialize<RSA>(rsaJson);
                }
                catch(Exception){
                    return (false, null);
                }
                return (true, rsa);
            }
            return (false, null);
        }

        public static bool WriteRSAFile(RSA toWrite, string path = "RSA.json", bool force = true)
        {
            string rsaJson = JsonSerializer.Serialize<RSA>(toWrite);
            return WriteFile(path, rsaJson, force);
        }

        #endregion

        #region WriteAndReadFile

        public static bool ReadFile(string path, out string res)
        {
            res = "";
            try
            {
                if (!File.Exists(path))
                    return false;
            }
            catch (Exception) { return false; }
            StreamReader sr = new StreamReader(path);
            res = sr.ReadToEnd();
            sr.Close();
            return true;
        }

        public static bool WriteFile(string path, string message, bool force)
        {
            try
            {
                if (!File.Exists(path) && !force)
                    return false;
            }
            catch (Exception) { return false; }
            StreamWriter sw = File.CreateText(path);
            sw.Write(message);
            sw.Close();
            return true;
        }

        public static List<(string, string)> ReadAllDirectory(string path){
            List<(string,string)> res = new List<(string, string)>();
            DirectoryInfo d = new DirectoryInfo(path);
            foreach(var file in d.EnumerateFiles()){
                StreamReader sr = new StreamReader(file.FullName);
                res.Add((file.Name ,sr.ReadToEnd()));
                sr.Close();
            }
            foreach(var dir in d.EnumerateDirectories()){
                foreach(var s in ReadAllDirectory(dir.FullName)){
                    res.Add((dir.Name + "/" + s.Item1, s.Item2));
                }
            }
            return res;
        }

        #endregion
    }
}
