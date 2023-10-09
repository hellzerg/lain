using System;

namespace Lain
{
    [Serializable]
    public sealed class LainAccount
    {
        string _Name;
        string _Email;
        string _Password;

        string _Link;
        string _Note;

        public LainAccount() { }

        public LainAccount(string name, string email, string password, string link, string note)
        {
            _Name = name;
            _Email = email;
            _Password = password;

            _Link = link;
            _Note = note;

            name = string.Empty;
            email = string.Empty;
            password = string.Empty;

            link = string.Empty;
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

        public string Link()
        {
            return _Link;
        }

        public string Note()
        {
            return _Note;
        }
    }
}
