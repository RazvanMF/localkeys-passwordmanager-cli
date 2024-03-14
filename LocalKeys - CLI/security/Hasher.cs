using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.security
{
    internal class Hasher
    {
        string username;
        SecureString password;
        
        public Hasher(string user, SecureString pass)
        {
            username = user;
            password = pass;
        }

        public string[] GetHashedCredentials()
        {
            SHA512 sha512 = SHA512.Create();
            byte[] sha512password = sha512.ComputeHash(Encoding.UTF8.GetBytes(new System.Net.NetworkCredential(string.Empty, password).Password));
            byte[] sha512username = sha512.ComputeHash(Encoding.UTF8.GetBytes(username));
            Rfc2898DeriveBytes element = new Rfc2898DeriveBytes(sha512password, sha512username, 100);
            byte[] passwordToStore = element.GetBytes(64);

            string[] credentials = new string[] {Convert.ToBase64String(sha512username), Convert.ToBase64String(passwordToStore)};
            return credentials;
        }
    }
}
