using LocalKeys___CLI.backend;
using LocalKeys___CLI.model;
using LocalKeys___CLI.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.view
{
    internal class CLI_UserDashboard
    {
        Token userToken;
        public CLI_UserDashboard(Token userToken)
        {
            this.userToken = userToken;
        }

        public void DisplayMenu()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            Console.WriteLine($"~~~~~ USER DASHBOARD FOR {userToken.Username} ~~~~~");
            Console.WriteLine("1. Display all accounts");
            Console.WriteLine("2. Add new account");
            Console.WriteLine("3. Delete an account");
            Console.WriteLine("4. Update an account");
            Console.WriteLine("0. Logout");
        }

        public Token AwaitCommand()
        {
            Console.Write("\n>>> ");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    DisplayPrompt();
                    List<Tuple<string, Account>> entries = CRUDController.HandleSelect(userToken.UserID, userToken.KeyAndIV);
                    DisplayAccounts(entries);

                    userToken.TokenState = ProgramState.DisplaySuccess;
                    return userToken;

                case "2":
                    AddPrompt();
                    Tuple<string, Account> entry = HarvestAddInformation();
                    bool addResult = CRUDController.HandleAdd(userToken.UserID, entry, userToken.KeyAndIV);
                    if (addResult) 
                    {
                        Console.WriteLine("Account added successfully.");
                        userToken.TokenState = ProgramState.AddSuccess;
                    }
                    else
                    {
                        Console.WriteLine("Add failed. Account already exists.");
                        userToken.TokenState = ProgramState.Fatal;
                    }

                    return userToken;


                case "3":
                    DeletePrompt();
                    Tuple<string, SecureString> deleteCredentials = HarvestDeleteValidation();
                    Hasher checker = new Hasher(userToken.Username, deleteCredentials.Item2);
                    if (checker.GetHashedCredentials()[1] == userToken.Hashedpass)
                    {
                        bool deleteResult = CRUDController.HandleDelete(userToken.UserID, deleteCredentials.Item1, userToken.KeyAndIV);
                        if (deleteResult)
                        {
                            Console.WriteLine("Account deleted successfully.");
                            userToken.TokenState = ProgramState.DeleteSuccess;
                        }
                        else
                        {
                            Console.WriteLine("Delete failed. Account does not exist.");
                            userToken.TokenState = ProgramState.DeleteFailure;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Delete failed. Master password does not match.");
                        userToken.TokenState= ProgramState.Fatal;
                    }

                    return userToken;

                case "4":
                    UpdatePrompt();
                    Tuple<string, SecureString> updateCredentials = HarvestUpdateValidation();
                    Hasher updateChecker = new Hasher(userToken.Username, updateCredentials.Item2);
                    if (updateChecker.GetHashedCredentials()[1] == userToken.Hashedpass)
                    {
                        Account credentialsToUpdate = HarvestUpdateInformation();
                        bool updateResult = CRUDController.HandleUpdate(userToken.UserID,
                            new Tuple<string, Account>(updateCredentials.Item1, credentialsToUpdate), userToken.KeyAndIV);
                        if (updateResult)
                        {
                            Console.WriteLine("Account updated successfully.");
                            userToken.TokenState = ProgramState.UpdateSuccess;
                        }
                        else
                        {
                            Console.WriteLine("Update failed. Account does not exist.");
                            userToken.TokenState = ProgramState.UpdateFailure;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Update failed. Master password does not match.");
                        userToken.TokenState = ProgramState.Fatal;
                    }

                    return userToken;

                case "0":
                    userToken.TokenState = ProgramState.Logout;
                    return userToken;

                default:
                    throw new Exception("Unexpected Input");
            }
        }

        public Tuple<string, Account> HarvestAddInformation()
        {
            string service;
            Account accountToAdd = new Account(null, null, null);

            Console.Write("Service for which you save credentials: ");
            service = Console.ReadLine();

            Console.Write("Email for the service (if an email is not needed, just press Enter): ");
            string email = Console.ReadLine();
            if (email != "")
                accountToAdd.Email = email;

            Console.Write("Username for the service (if an username is not needed, just press Enter): ");
            string username = Console.ReadLine();
            if (username != "")
                accountToAdd.Username = username;

            Console.Write("Password for the service: ");
            accountToAdd.Password = Console.ReadLine();

            Tuple<string, Account> credentials = new Tuple<string, Account>(service, accountToAdd);
            return credentials;
        }

        public Tuple<string, SecureString> HarvestDeleteValidation()
        {
            string service;
            Console.Write("Service for which you want to delete credentials: ");
            service = Console.ReadLine();

            Console.Write("DELETE OPERATION REQUIRES MASTER PASSWORD: ");
            SecureString password = PasswordReader.ReadPassword();

            Tuple<string, SecureString> credentials = new Tuple<string, SecureString>(service, password);
            return credentials;
        }

        public Tuple<string, SecureString> HarvestUpdateValidation()
        {
            string service;
            Console.Write("Service for which you want to update credentials: ");
            service = Console.ReadLine();

            Console.Write("UPDATE OPERATION REQUIRES MASTER PASSWORD: ");
            SecureString password = PasswordReader.ReadPassword();

            Tuple<string, SecureString> credentials = new Tuple<string, SecureString>(service, password);
            return credentials;
        }

        public Account HarvestUpdateInformation()
        {
            Account accountUpdateCredentials = new Account(null, null, null);

            Console.Write("New email for the service (if unchanged, press enter): ");
            string email = Console.ReadLine();
            if (email != "")
                accountUpdateCredentials.Email = email;

            Console.Write("New username for the service (if unchanged, press enter): ");
            string username = Console.ReadLine();
            if (username != "")
                accountUpdateCredentials.Username = username;

            Console.Write("New password for the service (if unchanged, press enter): ");
            string password = Console.ReadLine();
            if (password != "")
                accountUpdateCredentials.Password = password;

            return accountUpdateCredentials;

        }

        public void DisplayAccounts(List<Tuple<string, Account>> accounts)
        {
            foreach (Tuple<string, Account> acc in accounts)
            {
                Console.WriteLine($"{acc.Item1}:" +
                    $"\n\tEmail:{acc.Item2.Email}" +
                    $"\n\tUsername:{acc.Item2.Username}" +
                    $"\n\tPassword:{acc.Item2.Password}" +
                    $"\n");
            }
        }

        private void DisplayPrompt()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("DISPLAY ACCOUNTS:");
        }

        private void AddPrompt()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("ADD ACCOUNT:");
        }

        private void DeletePrompt()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("DELETE ACCOUNT:");
        }

        private void UpdatePrompt()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.WriteLine("UPDATE ACCOUNT:");
        }
    }
}
