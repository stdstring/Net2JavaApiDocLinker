using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UserDocsApiResolver
{
    public delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    public class KeyboardHook : IDisposable
    {
        public KeyboardHook(IUserRequestProcessor userRequestProcessor)
        {
            if (userRequestProcessor == null)
                throw new ArgumentNullException("userRequestProcessor");
            _userRequestProcessor = userRequestProcessor;
        }

        public void Dispose()
        {
            UnsetHook();
            GC.SuppressFinalize(this);
        }

        public void SetHook()
        {
            if (_hHook != IntPtr.Zero)
                return;
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
                _hHook = SetWindowsHookEx(WH_KEYBOARD_LL, HookProc, GetModuleHandle(curModule.ModuleName), 0);
            if (_hHook == IntPtr.Zero)
                throw new InvalidOperationException();
        }

        public void UnsetHook()
        {
            if (_hHook == IntPtr.Zero)
                return;
            UnhookWindowsHookEx(_hHook);
            _hHook = IntPtr.Zero;
        }

        public Boolean IsSetHook { get { return _hHook != IntPtr.Zero; } }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        ~KeyboardHook()
        {
            Dispose();
        }

        private IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                Int32 vkCode = Marshal.ReadInt32(lParam);
                if (Keys.D1 == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                    _userRequestProcessor.Process(UserActionKeys.CTRL_1_KEY);
                if (Keys.D2 == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                    _userRequestProcessor.Process(UserActionKeys.CTRL_2_KEY);
            }
            return CallNextHookEx(_hHook, nCode, wParam, lParam);
        }

        private const Int32 WH_KEYBOARD_LL = 13;
        private const Int32 WM_KEYDOWN = 0x0100;

        private IntPtr _hHook = IntPtr.Zero;
        private readonly IUserRequestProcessor _userRequestProcessor;
    }
}
