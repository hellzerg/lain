using System;

namespace Lain
{
    public sealed class LainAccount
    {
        string _Name;
        string _Email;
        string _Password;

        string _Link;
        string _Note;
        string _Meta;

        // decrypted metadata, filled on first use by AccountMeta.Read
        internal AccountMeta CachedMeta;

        // Decrypted values are deliberately not cached here. CLR strings cannot be reliably zeroed.
        internal string PlainName(VaultKey key) { return key.Decrypt(_Name); }
        internal string PlainEmail(VaultKey key) { return key.Decrypt(_Email); }
        internal string PlainLink(VaultKey key) { return key.Decrypt(_Link); }

        internal void Forget()
        {
            CachedMeta = null;
        }

        public LainAccount() { }

        public LainAccount(string name, string email, string password, string link, string note, string meta)
            : this(name, email, password, link, note)
        {
            _Meta = meta;
        }

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

        public string Meta()
        {
            return _Meta;
        }

        internal void SetMeta(string encrypted, AccountMeta meta)
        {
            _Meta = encrypted;
            CachedMeta = meta;
        }
    }
}
