using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using LocalKeys___CLI.security;

namespace LocalKeys___CLI.model
{
    internal class Token
    {
        public ProgramState TokenState { get; set; }
        public Deriver KeyAndIV { get; set; }
        public int? UserID { get; set; }
        public string Username { get; set; }
        public string Hasheduser { get; set; }
        public string Hashedpass { get; set; }
        public Token(ProgramState tokenState, Deriver kiv, int? userID = null, string username = null, string hasheduser = null, string hashedpass = null)
        {
            this.TokenState = tokenState;
            this.KeyAndIV = kiv;
            this.UserID = userID;

            this.Username = username;
            this.Hasheduser = hasheduser;
            this.Hashedpass = hashedpass;
        }
    }
}
