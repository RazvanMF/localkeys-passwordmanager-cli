using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using LocalKeys___CLI.model;
using static System.Net.Mime.MediaTypeNames;

namespace LocalKeys___CLI.security
{
    internal static class Encryptor
    {
        public static Account ReturnEncryptedAccount(this Account rawAccount, Deriver keyAndIV)
        {
            Account encryptedAccount = new Account(null, null, null);

            encryptedAccount.Email = (rawAccount.Email != null) ? HandleCredential(rawAccount.Email, keyAndIV) : null;
            encryptedAccount.Username = (rawAccount.Username != null) ? HandleCredential(rawAccount.Username, keyAndIV) : null;
            encryptedAccount.Password = (rawAccount.Password != null) ? HandleCredential(rawAccount.Password, keyAndIV) : null;

            return encryptedAccount;
        }

        private static string HandleCredential(string credentialToEncrypt, Deriver keyAndIV)
        {
            byte[] encryptedCredentialBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = keyAndIV.Key;
                aesAlg.IV = keyAndIV.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryStream))
                        {
                            writer.Write(credentialToEncrypt);
                        }
                        encryptedCredentialBytes = memStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encryptedCredentialBytes);
        }
    }
}
