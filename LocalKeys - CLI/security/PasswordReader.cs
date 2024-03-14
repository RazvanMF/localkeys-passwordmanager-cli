using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.security
{
    internal static class PasswordReader
    {
        public static SecureString ReadPassword()
        {
            SecureString password = new SecureString();
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

            return password;
        }
    }
}
