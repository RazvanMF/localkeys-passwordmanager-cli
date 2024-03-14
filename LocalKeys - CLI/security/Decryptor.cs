using LocalKeys___CLI.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.security
{
    internal static class Decryptor
    {
        public static Account ReturnDecryptedAccount(this Account encAccount, Deriver keyAndIV)
        {
            Account decryptedAccount = new Account(null, null, null);

            decryptedAccount.Email = (encAccount.Email != null) ? HandleCredential(encAccount.Email, keyAndIV) : null;
            decryptedAccount.Username = (encAccount.Username != null) ? HandleCredential(encAccount.Username, keyAndIV) : null;
            decryptedAccount.Password = (encAccount.Password != null) ? HandleCredential(encAccount.Password, keyAndIV) : null;

            return decryptedAccount;
        }

        private static string HandleCredential(string credentialToDecrypt, Deriver keyAndIV)
        {
            byte[] encryptedCredentialBytes;
            string decryptedCredential;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = keyAndIV.Key;
                aesAlg.IV = keyAndIV.IV;

                encryptedCredentialBytes = Convert.FromBase64String(credentialToDecrypt);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream memStream = new MemoryStream(encryptedCredentialBytes))
                {
                    using (CryptoStream cryStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryStream))
                        {
                            decryptedCredential = reader.ReadToEnd();
                        }
                    }
                }
            }
            return decryptedCredential;
        }
    }
}
