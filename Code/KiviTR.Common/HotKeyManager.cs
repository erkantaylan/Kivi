using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace KiviTR.Common
{
    public static class HotKeyManager
    {
        private static volatile MessageWindow wnd;
        private static volatile IntPtr windowHandle;
        private static readonly ManualResetEvent windowReadyEvent = new ManualResetEvent(false);

        private static int locationId;

        static HotKeyManager()
        {
            var messageLoop = new Thread(
                delegate() { Application.Run(new MessageWindow()); });
            messageLoop.Name = "MessageLoopThread";
            messageLoop.IsBackground = true;
            messageLoop.Start();
        }

        public static event EventHandler<HotKeyEventArgs> HotKeyPressed;


        public static int RegisterHotKey(Keys key, ModifierKeys modifiers)
        {
            windowReadyEvent.WaitOne();
            int id = Interlocked.Increment(ref locationId);
            wnd.Invoke(new RegisterHotKeyDelegate(RegisterHotKeyInternal), windowHandle, id, (uint) modifiers, (uint) key);
            return id;
        }

        public static void UnregisterHotKey(int id)
        {
            wnd.Invoke(new UnRegisterHotKeyDelegate(UnRegisterHotKeyInternal), windowHandle, id);
        }

        private static void RegisterHotKeyInternal(IntPtr hwnd, int id, uint modifiers, uint key)
        {
            RegisterHotKey(hwnd, id, modifiers, key);
        }

        private static void UnRegisterHotKeyInternal(IntPtr hwnd, int id)
        {
            UnregisterHotKey(windowHandle, id);
        }

        private static void OnHotKeyPressed(HotKeyEventArgs e)
        {
            HotKeyPressed?.Invoke(null, e);
        }

        [DllImport("user32", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private delegate void RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);

        private delegate void UnRegisterHotKeyDelegate(IntPtr hwnd, int id);

        private class MessageWindow : Form
        {
            private const int WM_HOTKEY = 0x312;

            public MessageWindow()
            {
                wnd = this;
                windowHandle = Handle;
                windowReadyEvent.Set();
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_HOTKEY)
                {
                    var e = new HotKeyEventArgs(m.LParam);
                    OnHotKeyPressed(e);
                }

                base.WndProc(ref m);
            }

            protected override void SetVisibleCore(bool value)
            {
                // Ensure the window never becomes visible
                base.SetVisibleCore(false);
            }
        }
    }

    public class HotKeyEventArgs : EventArgs
    {
        public readonly Keys Key;
        public readonly ModifierKeys Modifiers;

        public HotKeyEventArgs(Keys key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public HotKeyEventArgs(IntPtr hotKeyParam)
        {
            uint param = (uint) hotKeyParam.ToInt64();
            Key = (Keys) ((param & 0xffff0000) >> 16);
            Modifiers = (ModifierKeys) (param & 0x0000ffff);
        }
    }
}