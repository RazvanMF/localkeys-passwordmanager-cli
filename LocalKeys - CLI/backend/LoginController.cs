using LocalKeys___CLI.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.backend
{
    internal static class LoginController
    {
        static public int HandleLogin(Tuple<string, SecureString> rawCredentials)
        {
            SQLBridge bridge = new SQLBridge();
            Hasher hasher = new Hasher(rawCredentials.Item1, rawCredentials.Item2);
            string[] hashedCredentials = hasher.GetHashedCredentials();
            return bridge.SearchUserCredentials(hashedCredentials);
        }
    }
}
