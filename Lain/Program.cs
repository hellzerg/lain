using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        internal readonly static float Major = 4;
        internal readonly static float Minor = 0;

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
                                    FluentMessageBox.Show("You can't provide a salt, if the salt override file exists.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Environment.Exit(0);
                                }

                                string customSalt = arg.Replace("/salt=", string.Empty);
                                if (!SanitizeSalt(customSalt))
                                {
                                    FluentMessageBox.Show("Salt should be at least 32 characters.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Environment.Exit(0);
                                }

                                CryLain.SALT = customSalt;
                            }
                            catch (Exception err)
                            {
                                FluentMessageBox.Show(err.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Environment.Exit(0);
                            }
                        }

                        if (arg.StartsWith("/changesalt="))
                        {
                            if (!CryLain.VaultExists())
                            {
                                FluentMessageBox.Show("Lain hasn't need setup with a profile yet.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Environment.Exit(0);
                            }

                            if (CryLain.IsModernVault())
                            {
                                FluentMessageBox.Show("Your vault already uses a unique random salt, so changing it is no longer needed.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Environment.Exit(0);
                            }

                            string[] salts = arg.Replace("/changesalt=", string.Empty).Split(',');

                            // change default salt to a custom one
                            if (salts.Length == 2 && SanitizeSalt(salts[1]))
                            {
                                AuthorizeToChangeSalt(salts[0]);
                                ChangeSalt(salts[1], salts[0]);
                            }

                            // change custom salt, to a new one
                            else if (salts.Length == 3 && SanitizeSalt(salts[1]) && SanitizeSalt(salts[2]))
                            {
                                CryLain.SALT = salts[1];
                                AuthorizeToChangeSalt(salts[0]);
                                ChangeSalt(salts[2], salts[0]);
                            }
                            else
                            {
                                FluentMessageBox.Show("The arguments you provided are invalid.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Environment.Exit(0);
                            }
                        }
                    }

                    // custom SALT from file
                    try
                    {
                        if (File.Exists(Required.LainSalt))
                        {
                            string customSalt = File.ReadAllText(Required.LainSalt, Encoding.UTF8);
                            if (!SanitizeSalt(customSalt))
                            {
                                FluentMessageBox.Show("Salt should be at least 32 characters.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Environment.Exit(0);
                            }

                            CryLain.SALT = customSalt;
                        }
                    }
                    catch (Exception err)
                    {
                        FluentMessageBox.Show(err.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Environment.Exit(0);
                    }

                    if (!CryLain.VaultExists())
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
                    FluentMessageBox.Show("Lain is already running in the background!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Environment.Exit(0);
                }
            }
        }

        private static void ChangeSalt(string salt, string k)
        {
            if (CryLain.ChangeSalt(salt, k))
            {
                CryLain.SALT = salt;
                if (File.Exists(Required.LainSalt)) File.WriteAllText(Required.LainSalt, CryLain.SALT, Encoding.UTF8);
                FluentMessageBox.Show("Salt has been successfully changed.\n\nPlease restart Lain.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }
        }

        private static void AuthorizeToChangeSalt(string k)
        {
            if (!CryLain.VerifyLegacyPassword(k))
            {
                FluentMessageBox.Show("The password you provided is incorrect.\n\nLain will now exit.", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }
        }

        internal static void RestartLain()
        {
            // BYPASS SINGLE-INSTANCE MECHANISM
            if (Program.MUTEX != null)
            {
                Program.MUTEX.ReleaseMutex();
                Program.MUTEX.Dispose();
                Program.MUTEX = null;
            }

            // restart without command-line arguments
            ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Arguments = string.Empty;

            MethodInfo exit = typeof(Application).GetMethod("ExitInternal",
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Static);

            exit.Invoke(null, null);
            //Process.Start(startInfo);
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
            try
            {
                // nothing to save while the vault is locked; it was saved when it was locked
                if (MainForm.Key != null) CryLain.SaveVault(MainForm.Key, MainForm.Accounts);
            }
            catch (Exception ex)
            {
                FluentMessageBox.Show("Your accounts could not be saved.\n\n" + ex.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Options.SaveSettings();
            }
        }

        // Saves, then drops the vault key and decrypted-capable data from memory.
        internal static void CloseVault()
        {
            SaveSettings();
            if (MainForm.Key != null) MainForm.Key.Dispose();
            MainForm.Key = null;
            foreach (LainAccount a in MainForm.Accounts) a.Forget();
            MainForm.Accounts = new System.Collections.Generic.List<LainAccount>();
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
