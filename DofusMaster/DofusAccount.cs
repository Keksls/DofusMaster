using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WindowsInput.Native;
using WindowsInput;
using System.Threading;

namespace DofusMaster
{
    [Serializable]
    public class DofusAccount
    {
        #region Import windows low level API 
        const short SWP_NOMOVE = 0X2;
        const short SWP_NOSIZE = 1;
        const short SWP_NOZORDER = 0X4;
        const int SWP_SHOWWINDOW = 0x0040;
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(int dwProcessId);
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
        [DllImport("user32.dll")]
        static extern int MapVirtualKey(int uCode, uint uMapType);
        const uint MAPVK_VK_TO_CHAR = 0x02;
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_RCONTROL = 0xA3; //Right Control key code
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);
        public const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(IntPtr hWnd);
        #endregion

        public int Order { get; set; }
        public string Name { get; set; }
        public VirtualKeys SelectionKey { get; set; }
        public bool Selected { get; set; } = true;
        [JsonIgnore]
        public IntPtr Handle;
        [JsonIgnore]
        public Process Process;
        private InputSimulator sim;

        public DofusAccount() { }

        public DofusAccount(Process _process)
        {
            Process = _process;
            Handle = Process.MainWindowHandle;
            Name = Process.MainWindowTitle.Split(' ')[0];
            IO.KeyDown += IO_KeyDown;
            sim = new InputSimulator();
            AllowSetForegroundWindow(Process.Id);
        }

        public void SetOrder(int order)
        {
            Order = order;
            SelectionKey = VirtualKeys.Num1 + (byte)order;
        }

        public void DeregisterShortCut()
        {
            IO.KeyDown -= IO_KeyDown;
        }

        private void IO_KeyDown(VirtualKeys key)
        {
            if (key == SelectionKey)
                if ((SaveManager.Save.AccountShortcutCtrl && IO.isCtrl) || !SaveManager.Save.AccountShortcutCtrl)
                    ShowWindow();
        }

        public override string ToString()
        {
            return "(" + SelectionKey.ToString() + ") - " + Name;
        }

        public void ShowWindow()
        {
            SetForegroundWindow(Handle);
            Manager.SelectAccount(Order);
        }

        public void InstentMouseClick(int X, int Y)
        {
            int lparm = (Y << 16) + X;
            int lngResult = SendMessage(Handle.ToInt32(), WM_LBUTTONDOWN, 0, lparm);
            int lngResult2 = SendMessage(Handle.ToInt32(), WM_LBUTTONUP, 0, lparm);
        }

        public void SmoothMouseClick(int X, int Y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)X, (uint)Y, 0, 0);
        }

        public void KeyPress(VirtualKeyCode key)
        {
            sim.Keyboard.KeyPress(key);
        }

        public void KeyDown(VirtualKeyCode key)
        {
            sim.Keyboard.KeyDown(key);
        }

        public void KeyUp(VirtualKeyCode key)
        {
            sim.Keyboard.KeyUp(key);
        }

        public void WriteIntoConsole(string text, bool setFocusBefore = true, bool sendAfterWrite = true)
        {
            if (setFocusBefore)
                ShowWindow();
            Thread.Sleep(100);

            KeyPress(VirtualKeyCode.SPACE);
            Thread.Sleep(50);

            foreach (char ch in text)
            {
                sim.Keyboard.TextEntry(ch);
                Thread.Sleep(20);
            }

            Thread.Sleep(100);

            if (sendAfterWrite)
                KeyPress(VirtualKeyCode.RETURN);

            Thread.Sleep(100);
        }

        public void AutoPilote(int x, int y)
        {
            WriteIntoConsole("/travel " + x + " " + y + "");
            Thread.Sleep(1500);
            KeyPress(VirtualKeyCode.RETURN);
        }
    }
}