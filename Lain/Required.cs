using System;
using System.IO;
using System.Windows.Forms;

namespace Lain
{
    internal sealed class Required
    {
        internal readonly static string DataFolder = Path.Combine(Application.StartupPath, "Data");
        internal readonly static string LainSerial = Path.Combine(DataFolder, "Lain.serial");
        internal readonly static string LainData = Path.Combine(DataFolder, "Lain.data");
        internal readonly static string LainSalt = Path.Combine(DataFolder, "Lain.salt");

        internal static void Deploy()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                {
                    Directory.CreateDirectory(DataFolder);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }
        }
    }
}
