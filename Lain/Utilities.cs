using System.Collections.Generic;
using System.Windows.Forms;

namespace Lain
{
    internal static class Utilities
    {
        internal static IEnumerable<Control> GetSelfAndChildrenRecursive(Control parent)
        {
            List<Control> controls = new List<Control>();

            foreach (Control child in parent.Controls)
            {
                controls.AddRange(GetSelfAndChildrenRecursive(child));
            }

            controls.Add(parent);
            return controls;
        }

        //internal static string RetrieveLink(string text)
        //{
        //    string result = string.Empty;

        //    IEnumerable<string> links = text.Split("\t\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => s.StartsWith("http://") || s.StartsWith("www.") || s.StartsWith("https://"));
        //    foreach (string link in links)
        //    {
        //        if (!string.IsNullOrEmpty(link))
        //        {
        //            result = link;
        //            break;
        //        }
        //    }

        //    return result;
        //}
    }
}
