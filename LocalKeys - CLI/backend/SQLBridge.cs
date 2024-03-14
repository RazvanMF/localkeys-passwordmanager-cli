using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LocalKeys___CLI.backend;
using LocalKeys___CLI.model;
using LocalKeys___CLI.security;
using System.Net;

namespace LocalKeys___CLI.backend
{
    internal class SQLBridge
    {
        public SQLBridge() { }

        private string connectionString = @"Data Source=DESKTOP-MAIN;" +
                                          @"Initial Catalog=LocalKeys;" +
                                          @"Integrated Security=true;";

        public bool WriteUserCredentials(string[] credentials)
        {
            //credentials[0] is expected to be the username, and credentials[1] is expected to be the password
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                DataSet userInformation_DataSet = new DataSet();
                SqlDataAdapter userInformation_DataAdapter = new SqlDataAdapter("SELECT * FROM [UserInformation]", conn);
                SqlCommandBuilder userInformation_CMDBuilder = new SqlCommandBuilder(userInformation_DataAdapter);

                userInformation_DataAdapter.Fill(userInformation_DataSet, "UserInformation");
                foreach (DataRow row in userInformation_DataSet.Tables["UserInformation"].Rows)
                {
                    if ((string)row["Username"] == credentials[0])
                    {
                        return false;
                    }
                }

                DataRow newRow = userInformation_DataSet.Tables["UserInformation"].NewRow();
                newRow["Username"] = credentials[0];
                newRow["Password"] = credentials[1];

                userInformation_DataSet.Tables["UserInformation"].Rows.Add(newRow);
                userInformation_DataAdapter.Update(userInformation_DataSet, "UserInformation");

                return true;

            }
        }

        public int SearchUserCredentials(string[] hashedCredentials)
        {
            //credentials[0] is expected to be the username, and credentials[1] is expected to be the password
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                DataSet userInformation_DataSet = new DataSet();
                SqlDataAdapter userInformation_DataAdapter = new SqlDataAdapter("SELECT * FROM [UserInformation]", conn);
                SqlCommandBuilder userInformation_CMDBuilder = new SqlCommandBuilder(userInformation_DataAdapter);

                userInformation_DataAdapter.Fill(userInformation_DataSet, "UserInformation");
                foreach (DataRow row in userInformation_DataSet.Tables["UserInformation"].Rows)
                {
                    if ((string)row["Username"] == hashedCredentials[0] && (string)row["Password"] == hashedCredentials[1])
                    {
                        return (int)row["userID"];
                    }
                }
                return -1;
            }
        }

        public bool AddAccount(int userID, Tuple<string, Account> credentials)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                DataSet accounts_DataSet = new DataSet();
                SqlCommand selectCMD = new SqlCommand("SELECT * FROM [Accounts] WHERE userID = @uID", conn);
                selectCMD.Parameters.Add("@uID", SqlDbType.Int).Value = userID;

                SqlDataAdapter accounts_DataAdapter = new SqlDataAdapter(selectCMD);
                SqlCommandBuilder accounts_CMDBuilder = new SqlCommandBuilder(accounts_DataAdapter);

                accounts_DataAdapter.Fill(accounts_DataSet, "Accounts");
                foreach (DataRow row in accounts_DataSet.Tables["Accounts"].Rows)
                {
                    if ((string)row["service"] == credentials.Item1)
                    {
                        return false;
                    }
                }

                DataRow newRow = accounts_DataSet.Tables["Accounts"].NewRow();
                newRow["userID"] = userID;
                newRow["Service"] = credentials.Item1;
                newRow["Email"] = credentials.Item2.Email;
                newRow["Username"] = credentials.Item2.Username;
                newRow["Password"] = credentials.Item2.Password;

                accounts_DataSet.Tables["Accounts"].Rows.Add(newRow);
                accounts_DataAdapter.Update(accounts_DataSet, "Accounts");

                return true;
            }
        }

        public List<Tuple<string, Account>> GetCredentials(int userID)
        {
            List<Tuple<string, Account>> encryptedEntries = new List<Tuple<string, Account>>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                DataSet accounts_DataSet = new DataSet();
                SqlCommand selectCMD = new SqlCommand("SELECT * FROM [Accounts] WHERE userID = @uID", conn);
                selectCMD.Parameters.Add("@uID", SqlDbType.Int).Value = userID;

                SqlDataAdapter accounts_DataAdapter = new SqlDataAdapter(selectCMD);
                SqlCommandBuilder accounts_CMDBuilder = new SqlCommandBuilder(accounts_DataAdapter);

                accounts_DataAdapter.Fill(accounts_DataSet, "Accounts");
                foreach (DataRow row in accounts_DataSet.Tables["Accounts"].Rows)
                {
                    Account acc = new Account(null, null, null);
                    acc.Email = (row["email"] != System.DBNull.Value) ? (string)row["email"] : null;
                    acc.Username = (row["username"] != System.DBNull.Value) ? (string)row["username"] : null;
                    acc.Password = (row["password"] != System.DBNull.Value) ? (string)row["password"] : null;
                    encryptedEntries.Add(new Tuple<string, Account>((string)row["service"], acc));
                }
            }
            return encryptedEntries;
        }

        public bool DeleteAccount(int userID, string service)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                DataSet accounts_DataSet = new DataSet();
                SqlCommand selectCMD = new SqlCommand("SELECT * FROM [Accounts] WHERE userID = @uID AND service = @serv", conn);
                selectCMD.Parameters.Add("@uID", SqlDbType.Int).Value = userID;
                selectCMD.Parameters.Add("@serv", SqlDbType.NVarChar).Value = service;

                SqlDataAdapter accounts_DataAdapter = new SqlDataAdapter(selectCMD);
                SqlCommandBuilder accounts_CMDBuilder = new SqlCommandBuilder(accounts_DataAdapter);

                accounts_DataAdapter.Fill(accounts_DataSet, "Accounts");

                if (accounts_DataSet.Tables["Accounts"].Rows.Count == 0)
                    return false;

                foreach (DataRow row in accounts_DataSet.Tables["Accounts"].Rows)
                    row.Delete();

                accounts_DataAdapter.Update(accounts_DataSet, "Accounts");

                return true;
            }
        }

        public bool UpdateAccount(int userID, string serviceToModify, Account encryptedNewEntry)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                DataSet accounts_DataSet = new DataSet();
                SqlCommand selectCMD = new SqlCommand("SELECT * FROM [Accounts] WHERE userID = @uID AND service = @serv", conn);
                selectCMD.Parameters.Add("@uID", SqlDbType.Int).Value = userID;
                selectCMD.Parameters.Add("@serv", SqlDbType.NVarChar).Value = serviceToModify;

                SqlDataAdapter account_DataAdapter = new SqlDataAdapter(selectCMD);
                SqlCommandBuilder account_CMDBuilder = new SqlCommandBuilder(account_DataAdapter);

                account_DataAdapter.Fill(accounts_DataSet, "Accounts");

                if (accounts_DataSet.Tables["Accounts"].Rows.Count == 0)
                    return false;

                DataRow rowToModify = accounts_DataSet.Tables["Accounts"].Rows[0];
                rowToModify["email"] = (encryptedNewEntry.Email != null) ? encryptedNewEntry.Email : rowToModify["email"];
                rowToModify["username"] = (encryptedNewEntry.Username != null) ? encryptedNewEntry.Username : rowToModify["username"];
                rowToModify["password"] = (encryptedNewEntry.Password != null) ? encryptedNewEntry.Password : rowToModify["password"];

                account_DataAdapter.Update(accounts_DataSet, "Accounts");
                return true;
            }
        }
    }
}
