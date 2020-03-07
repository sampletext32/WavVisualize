using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WavVisualize
{
    public class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;

            public override string ToString()
            {
                return handle + ", " + msg + ", " + wParam + ", " + lParam + ", " + time + ", " + p;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr window, uint wMsgFilterMin,
            uint wMsgFilterMax, uint wRemoveMsg);

        public static bool AppIsIdle()
        {
            return !PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
        }
    }
}