using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lain
{
    static class Program
    {
        /* VERSION PROPERTIES */
        /* DO NOT LEAVE THEM EMPTY */

        // Enter current version here
        internal readonly static float Major = 1;
        internal readonly static float Minor = 9;

        /* END OF VERSION PROPERTIES */

        static ApplicationContext _mainContext = new ApplicationContext();

        const string _jsonAssembly = @"Lain.Newtonsoft.Json.dll";

        internal static string GetCurrentVersion()
        {
            return Major.ToString() + "." + Minor.ToString();
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EmbeddedAssembly.Load(_jsonAssembly, _jsonAssembly.Replace("Lain.", string.Empty));
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            if (!Directory.Exists(Required.DataFolder))
            {
                Required.Deploy();
            }

            Options.LoadSettings();

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
