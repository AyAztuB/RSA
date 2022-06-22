using System;
using System.IO;
using System.Text.Json;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }


        #region Option

        public static void DisplayHelp(string option = "")
        {
            Console.WriteLine("This program can create RSA keys, decrypt and encrypt messages using the RSA crypto-system and can break some very small RSA keys (only in source code) !\n");
            System.Console.WriteLine("run <OPTION> [-h] [-c pathToCommonKeyPart] [-p keyPath] [-m message] [-f File] [-d Directory] [-o filename] [-od dirname] [-j useJson]");
            Console.WriteLine();
            if (option == "")
            {
                System.Console.WriteLine("Available OPTION :");
                System.Console.WriteLine();
                System.Console.WriteLine("--REPL        \t=> Currently in developpement !\n");
                System.Console.WriteLine("--GenerateKeys\t=> Generate public and private key to write them in a [json] file\n");
                System.Console.WriteLine("--Encrypt     \t=> Use key file to encrypt message/file/directory\n");
                System.Console.WriteLine("--Decrypt     \t=> Use key file to decrypt message/file/directory\n");
            }
        }

        #endregion


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
