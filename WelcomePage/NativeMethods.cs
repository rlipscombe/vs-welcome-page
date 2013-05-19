using System;
using System.Runtime.InteropServices;

namespace RogerLipscombe.WelcomePage
{
    internal static class NativeMethods
    {
        public static uint WM_QUIT = 0x12;
        public static uint GW_NEXT = 2;

        [DllImport("user32.dll", SetLastError = true)]
        internal extern static IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal extern static uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal extern static bool PostMessage(HandleRef hWnd, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal extern static IntPtr GetWindow(IntPtr hWnd, uint uCmd);
    }
}