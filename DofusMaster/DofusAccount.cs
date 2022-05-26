using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DofusMaster
{
    public class DofusAccount
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);
        public const int SW_RESTORE = 9;

        public IntPtr Handle;
        public string Name;
        public Process Process;

        public DofusAccount(Process _process)
        {
            Process = _process;
            Handle = Process.MainWindowHandle;
            Name = Process.MainWindowTitle.Split(' ')[0];
        }

        public override string ToString()
        {
            return Name;
        }

        public void ShowWindow()
        {
            SetForegroundWindow(Handle);
        }
    }
}