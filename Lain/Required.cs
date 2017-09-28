using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lain
{
    public class Required
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
