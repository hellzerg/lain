using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lain
{
    // Copies secrets to the clipboard so they stay out of Windows clipboard history / cloud sync,
    // and removes them again after a timeout - but only if the clipboard still holds what we put there.
    internal static class SecureClipboard
    {
        internal const int MinSeconds = 5;
        internal const int MaxSeconds = 600;

        [DllImport("user32.dll")]
        static extern uint GetClipboardSequenceNumber();

        static Timer _timer;
        static uint _sequence;
        static bool _owned;

        internal static int ClampSeconds(int seconds)
        {
            return Math.Max(MinSeconds, Math.Min(MaxSeconds, seconds));
        }

        internal static bool AutoClear
        {
            get { return Options.CurrentOptions.ClearClipboard; }
        }

        internal static int Seconds
        {
            get { return ClampSeconds(Options.CurrentOptions.ClipboardSeconds); }
        }

        internal static void CopySecret(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            DataObject data = new DataObject();
            data.SetData(DataFormats.UnicodeText, text);
            data.SetData(DataFormats.Text, text);
            data.SetData("ExcludeClipboardContentFromMonitorProcessing", new MemoryStream(new byte[] { 0, 0, 0, 0 }));
            data.SetData("CanIncludeInClipboardHistory", new MemoryStream(BitConverter.GetBytes(0)));
            data.SetData("CanUploadToCloudClipboard", new MemoryStream(BitConverter.GetBytes(0)));

            Clipboard.SetDataObject(data, true, 5, 100);

            _sequence = GetClipboardSequenceNumber();
            _owned = true;

            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Tick += (s, e) => ClearIfOurs();
            }
            _timer.Stop();
            if (AutoClear)
            {
                _timer.Interval = Seconds * 1000;
                _timer.Start();
            }
        }

        // Clears the clipboard only when it still contains the secret we copied (nothing was copied since).
        internal static void ClearIfOurs()
        {
            if (_timer != null) _timer.Stop();
            if (!_owned) return;
            _owned = false;

            try
            {
                if (GetClipboardSequenceNumber() == _sequence) Clipboard.Clear();
            }
            catch { }
        }
    }
}
