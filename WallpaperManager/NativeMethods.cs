using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace WallpaperManager
{
    class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);


        public const int MONITORINFOF_PRIMARY = 0x1;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        public const int EDD_GET_DEVICE_INTERFACE_NAME = 0x1;
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern bool EnumDisplayDevices(string lpDevice, int iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);

        public const int DISPLAY_DEVICE_ACTIVE = 0x00000001;
        public const int DISPLAY_DEVICE_MIRRORING_DRIVER = 0x00000008;
        public const int DISPLAY_DEVICE_VGA_COMPATIBLE = 0x00000010;
        public const int DISPLAY_DEVICE_REMOVABLE = 0x00000020;
        public const int DISPLAY_DEVICE_MODESPRUNED = 0x08000000;
        public const int DISPLAY_DEVICE_PRIMARY_DEVICE = 0x00000004;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DISPLAY_DEVICE
        {
            public int cb;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;

            public int StateFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        public const int SM_CMONITORS = 80;

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern bool SendNotifyMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}
