using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.model
{
    internal class Account
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Account(string email, string username, string pass) 
        {
            Email = email;
            Username = username;
            Password = pass;
        }
    }
}
