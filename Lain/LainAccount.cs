using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lain
{
    [Serializable]
    public class LainAccount
    {
        string _name;
        string _email;
        string _password;
        string _note;

        public LainAccount()
        {

        }

        public LainAccount(string name, string email, string password, string note)
        {
            _name = name;
            _email = email;
            _password = password;
            _note = note;

            name = string.Empty;
            email = string.Empty;
            password = string.Empty;
            note = string.Empty;
        }

        public string Name()
        {
            return _name;
        }

        public string Email()
        {
            return _email;
        }

        public string Password()
        {
            return _password;
        }

        public string Note()
        {
            return _note;
        }
    }
}
