using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Lain
{
    static class Program
    {
        /* VERSION PROPERTIES */
        /* DO NOT LEAVE THEM EMPTY */

        // Enter current version here
        internal readonly static float Major = 2;
        internal readonly static float Minor = 9;

        /* END OF VERSION PROPERTIES */

        static ApplicationContext _mainContext = new ApplicationContext();

        const string _jsonAssembly = @"Lain.Newtonsoft.Json.dll";

        internal static string GetCurrentVersionToString()
        {
            return Major.ToString() + "." + Minor.ToString();
        }

        internal static float GetCurrentVersion()
        {
            return float.Parse(GetCurrentVersionToString());
        }

        const string _mutexGuid = @"{DEADMOON-1EFD7B9A-D5FD-677G-B4B3-0118C612AE19-LAIN}";
        internal static Mutex MUTEX;
        static bool _notRunning;

        [STAThread]
        static void Main(string[] switches)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EmbeddedAssembly.Load(_jsonAssembly, _jsonAssembly.Replace("Lain.", string.Empty));
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            using (MUTEX = new Mutex(true, _mutexGuid, out _notRunning))
            {
                if (_notRunning)
                {
                    if (!Directory.Exists(Required.DataFolder))
                    {
                        Required.Deploy();
                    }

                    Options.LoadSettings();

                    // custom SALT from command-line
                    if (switches.Length == 1)
                    {
                        string arg = switches[0].Trim();
                        if (arg.StartsWith("/salt="))
                        {
                            try
                            {
                                if (File.Exists(Required.LainSalt))
                                {
                                    MessageBox.Show("You can't provide a salt, if the salt override file exists.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Environment.Exit(0);
                                }

                                string customSalt = arg.Replace("/salt=", string.Empty);
                                if (!SanitizeSalt(customSalt))
                                {
                                    MessageBox.Show("Salt should be at least 32 characters.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Environment.Exit(0);
                                }

                                CryLain.SALT = customSalt;
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show(err.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Environment.Exit(0);
                            }
                        }
                    }

                    // custom SALT from file
                    try
                    {
                        if (File.Exists(Required.LainSalt))
                        {
                            if (MessageBox.Show("Salt override file detected. Do you want to use it?", "Lain", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                string customSalt = File.ReadAllText(Required.LainSalt, Encoding.UTF8);
                                if (!SanitizeSalt(customSalt))
                                {
                                    MessageBox.Show("Salt should be at least 32 characters.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Environment.Exit(0);
                                }

                                CryLain.SALT = customSalt;
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Environment.Exit(0);
                    }

                    if (!File.Exists(Required.LainSerial))
                    {
                        _mainContext.MainForm = new WizardForm();
                    }
                    else
                    {
                        _mainContext.MainForm = new LoginForm(LoginType.Login);
                    }

                    Application.Run(_mainContext);
                }
                else
                {
                    MessageBox.Show("Lain is already running in the background!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Environment.Exit(0);
                }
            }
        }

        private static bool SanitizeSalt(string salt)
        {
            return !string.IsNullOrEmpty(salt) && salt.Length >= 32;
        }

        internal static void SetMainForm(Form form)
        {
            _mainContext.MainForm = form;
            form.FormClosed += form_FormClosed;
        }

        private static void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }

        internal static void SaveSettings()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(ms, MainForm.Accounts);
            byte[] bytes = ms.ToArray();

            try
            {
                File.WriteAllBytes(Required.LainData, bytes);
            }
            finally
            {
                bytes = null;
                ms = null;

                Options.SaveSettings();
            }
        }

        internal static void ShowMainForm()
        {
            _mainContext.MainForm.Show();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
