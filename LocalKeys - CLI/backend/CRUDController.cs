using LocalKeys___CLI.model;
using LocalKeys___CLI.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.backend
{
    internal static class CRUDController
    {
        static public bool HandleAdd(int? userID, Tuple<string, Account> entry, Deriver kiv)
        {
            if (userID == null)
                throw new ArgumentNullException(nameof(userID));

            SQLBridge bridge = new SQLBridge();
            Account encryptedAccount = entry.Item2.ReturnEncryptedAccount(kiv);
            Tuple<string, Account> encryptedEntry = new Tuple<string, Account>(entry.Item1, encryptedAccount);

            return bridge.AddAccount((int)userID, encryptedEntry);
        }

        static public List<Tuple<string, Account>> HandleSelect(int? userID, Deriver kiv)
        {
            if (userID == null)
                throw new ArgumentNullException(nameof(userID));

            SQLBridge bridge = new SQLBridge();
            List<Tuple<string, Account>> encryptedEntries = bridge.GetCredentials((int)userID);
            List<Tuple<string, Account>> decryptedEntries = new List<Tuple<string, Account>>();

            foreach (Tuple<string, Account> entry in encryptedEntries)
            {
                Account decryptedAccount = entry.Item2.ReturnDecryptedAccount(kiv);
                Tuple<string, Account> decryptedEntry = new Tuple<string, Account>(entry.Item1, decryptedAccount);
                decryptedEntries.Add(decryptedEntry);
            }

            return decryptedEntries;
        }

        static public bool HandleDelete(int? userID, string service, Deriver kiv)
        {
            //we search by service name, so we don't need encryption/decryption. kiv is a parameter for symmetry
            if (userID == null)
                throw new ArgumentNullException(nameof(userID));

            SQLBridge bridge = new SQLBridge();
            return bridge.DeleteAccount((int)userID, service);
        }

        static public bool HandleUpdate(int? userID, Tuple<string, Account> newEntry, Deriver kiv)
        {
            if (userID == null)
                throw new ArgumentNullException(nameof(userID));

            SQLBridge bridge = new SQLBridge();
            Account encryptedAccount = newEntry.Item2.ReturnEncryptedAccount(kiv);
            Tuple<string, Account> encryptedEntry = new Tuple<string, Account>(newEntry.Item1, encryptedAccount);

            return bridge.UpdateAccount((int)userID, newEntry.Item1, encryptedAccount);
        }
    }
}
