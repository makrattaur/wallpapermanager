using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Runtime.InteropServices;

namespace WallpaperManager
{
    class Utils
    {
        public static Monitor[] GetMonitors()
        {
            List<Monitor> monitors = new List<Monitor>();

            NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref NativeMethods.RECT lprcMonitor, IntPtr dwData) =>
                {
                    NativeMethods.MONITORINFOEX monitorInfo = new NativeMethods.MONITORINFOEX();
                    monitorInfo.cbSize = Marshal.SizeOf(typeof(NativeMethods.MONITORINFOEX));

                    NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);

                    monitors.Add(new Monitor()
                    {
                        Primary = (monitorInfo.dwFlags & NativeMethods.MONITORINFOF_PRIMARY) != 0,
                        DeviceName = monitorInfo.szDevice,
                        Rect = Rectangle.FromLTRB(monitorInfo.rcMonitor.Left,
                            monitorInfo.rcMonitor.Top,
                            monitorInfo.rcMonitor.Right,
                            monitorInfo.rcMonitor.Bottom),
                        WorkRect = Rectangle.FromLTRB(monitorInfo.rcWork.Left,
                            monitorInfo.rcWork.Top,
                            monitorInfo.rcWork.Right,
                            monitorInfo.rcWork.Bottom)
                    });

                    return true;
                },
                IntPtr.Zero
            );

            return monitors.ToArray();
        }

        public static void EnsureSmoothTransition()
        {
            IntPtr window = NativeMethods.FindWindow("Progman", null);
            if (window != IntPtr.Zero)
            {
                // 0x52c = WM_USER + 300
                NativeMethods.SendNotifyMessage(window, 0x52c, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
