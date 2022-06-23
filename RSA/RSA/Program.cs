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
            if (common != "" && common[^1] == '/')
                common += "common";
            if (key != ""){
                if (key[^1] == '/')
                    key += "rsa";
                else if(key.Split('/')[^1].Contains('.')){
                    Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("FAIL: THE GIVEN KEY FILE HAVE AN EXTENSION !");
                    Console.ForegroundColor = ConsoleColor.White;
                    DisplayHelp(help);
                    return;
                }
            }
            if (json != ""){
                if (json[^1] == '/')
                    json += "RSA.json";
                else {
                    string last = json.Split('/')[^1];
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
        #endregion

        #endregion


        private static bool IsPathCorrect(string path){
            if (path[^1] == '/')
                return Directory.Exists(path);
            return Directory.Exists(GetFileDir(path));
        }

        private static string GetFileDir(string path){
            string[] split = path.Split('/');
            string fileDir = "";
            for (int i = 0; i < split.Length -1; i++){
                fileDir += (split[i] + "/");
            }
            return fileDir;
        }

        private static string GetCompletePath(string path){
            if (path[0] == '/')
                return path;
            string[] execDir = (new DirectoryInfo("./")).FullName.Split('/');
            string[] pathSplit = path.Split('/');
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
            foreach(var i in execDir){
                res += ("/" + i);
            }
            return res;
        }


        #region RSA_from_json

        public static (bool, RSA) ReadRSAFile(string path = "RSA.json")
        {
            string rsaJson = "";
            if (ReadFile(path, out rsaJson))
                return (true, JsonSerializer.Deserialize<RSA>(rsaJson));
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

        #endregion
    }
}
