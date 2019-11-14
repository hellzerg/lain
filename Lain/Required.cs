using System.IO;
using System.Windows.Forms;

namespace Lain
{
    internal class Required
    {
        internal readonly static string DataFolder = Application.StartupPath + "\\Data\\";
        internal readonly static string LainSerial = DataFolder + "Lain.Serial";
        internal readonly static string LainData = DataFolder + "Lain.Data";

        internal static void Deploy()
        {
            try
            {
                if (!Directory.Exists(DataFolder))
                {
                    Directory.CreateDirectory(DataFolder);
                }
            }
            catch { }
        }
    }
}
