using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.security
{
    internal class Deriver
    {
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }

        public Deriver(string username, SecureString password)
        {
            generateEncryptionBytes(username, password);
        }

        private void generateEncryptionBytes(string username, SecureString password)
        {
            if (username.Count() < 8)
                username = username.PadRight(8, '0');

            Rfc2898DeriveBytes rfcderiver = new Rfc2898DeriveBytes(new System.Net.NetworkCredential(string.Empty, password).Password, Encoding.UTF8.GetBytes(username));
            Key = rfcderiver.GetBytes(32);
            IV = rfcderiver.GetBytes(16);
        }

    }
}
