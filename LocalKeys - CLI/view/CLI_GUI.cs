using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using LocalKeys___CLI.backend;
using LocalKeys___CLI.security;
using LocalKeys___CLI.model;

namespace LocalKeys___CLI.view
{
    internal class CLI_GUI
    {
        public CLI_GUI() { }
        public void DisplayMenu() 
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            Console.WriteLine("~~~~~ WELCOME TO LOCALKEYS ~~~~~");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("0. Exit");
        }

        public Token AwaitCommand()
        {
            Console.Write("\n>>> "); 
            string input = Console.ReadLine();
            switch(input)
            {
                case "1":
                    LoginPrompt();
                    Tuple<string, SecureString> loginInformation = HarvestRawInformation();
                    int userID = LoginController.HandleLogin(loginInformation);
                    if (userID != -1)
                    {
                        Hasher hasher = new Hasher(loginInformation.Item1, loginInformation.Item2);
                        Deriver deriver = new Deriver(loginInformation.Item1, loginInformation.Item2);
                        Token loginToken = new Token(
                            ProgramState.LoginSuccess,
                            deriver,
                            userID,
                            loginInformation.Item1,
                            hasher.GetHashedCredentials()[0],
                            hasher.GetHashedCredentials()[1]);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("SUCCESS - You will now enter the user dashboard");
                        return loginToken;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FAILURE - Invalid login credentials.");
                        return new Token(ProgramState.LoginFailure, null);
                    }
                    
                case "2":
                    RegistrationPrompt();
                    Tuple<string, SecureString> registerInformation = HarvestRawInformation();
                    if (RegistrationController.HandleRegistration(registerInformation))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("SUCCESS - Account successfully registered.");
                        return new Token(ProgramState.RegistrationSuccess, null);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FAILURE - This username already exists.");
                        return new Token(ProgramState.RegistrationFailure, null);
                    }
                case "0":
                    System.Environment.Exit(0);
                    break;
                default:
                    throw new Exception("Unexpected Input");
            }

            return new Token(ProgramState.Fatal, null);  // this should NEVER be reached
        }

        private void LoginPrompt()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("LOGIN:");
        }

        private void RegistrationPrompt()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("REGISTER:");
        }

        private Tuple<string, SecureString> HarvestRawInformation()
        {
            string username = null;
            SecureString password = new SecureString();

            Console.Write("Username: ");
            username = Console.ReadLine();

            Console.Write("Password: ");
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && password.Length >= 1)
                {
                    password.RemoveAt(password.Length - 1);
                    Console.Write("\b \b");

                }

                if ((key.Key >= ConsoleKey.A && key.Key <= ConsoleKey.Z) ||
                    (key.Key >= ConsoleKey.D0 && key.Key <= ConsoleKey.D9))
                {
                    password.AppendChar(key.KeyChar);
                    Console.Write('*');
                }

            } while (key.Key != ConsoleKey.Enter);
            password.MakeReadOnly();
            Console.WriteLine();

            //new System.Net.NetworkCredential(string.Empty, password).Password
            Tuple<string, SecureString> credentials = new Tuple<string, SecureString>(username, password);
            return credentials;
        }
    }
}
