using System;

namespace Lain
{
    [Serializable]
    public class LainAccount
    {
        string _Name;
        string _Email;
        string _Password;
        string _Note;

        public LainAccount()
        {

        }

        public LainAccount(string name, string email, string password, string note)
        {
            _Name = name;
            _Email = email;
            _Password = password;
            _Note = note;

            name = string.Empty;
            email = string.Empty;
            password = string.Empty;
            note = string.Empty;
        }

        public string Name()
        {
            return _Name;
        }

        public string Email()
        {
            return _Email;
        }

        public string Password()
        {
            return _Password;
        }

        public string Note()
        {
            return _Note;
        }
    }
}
