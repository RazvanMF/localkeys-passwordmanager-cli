using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Security;
using System.Data.SqlClient;
using System.Data;

using LocalKeys___CLI.view;
using LocalKeys___CLI.backend;
using LocalKeys___CLI.security;
using LocalKeys___CLI.model;

namespace LocalKeys___CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CLI_GUI menu = new CLI_GUI();
            while (true)
            {
                menu.DisplayMenu();
                Token result = menu.AwaitCommand();
                if (result.TokenState == ProgramState.LoginSuccess)
                {
                    KeepOpen();
                    CLI_UserDashboard userDashboard = new CLI_UserDashboard(result);
                    while (true)
                    {
                        userDashboard.DisplayMenu();
                        Token userbasedResult = userDashboard.AwaitCommand();
                        if (userbasedResult.TokenState == ProgramState.Logout)
                            break;
                        KeepOpen();
                    }
                }
                KeepOpen();
            }


        }

        static void KeepOpen()
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey();
            }
        }
    }
}
