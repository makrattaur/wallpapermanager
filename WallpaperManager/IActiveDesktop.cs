using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace WallpaperManager
{
    [Flags]
    public enum AD_Apply : int
    {
        SAVE = 0x00000001,
        HTMLGEN = 0x00000002,
        REFRESH = 0x00000004,
        ALL = SAVE | HTMLGEN | REFRESH,
        FORCE = 0x00000008,
        BUFFERED_REFRESH = 0x00000010,
        DYNAMICREFRESH = 0x00000020
    }

    public enum WallpaperStyle : int
    {
        WPSTYLE_CENTER = 0,
        WPSTYLE_TILE = 1,
        WPSTYLE_STRETCH = 2,
        WPSTYLE_MAX = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WALLPAPEROPT
    {
        public int dwSize;     // size of this Structure.
        public WallpaperStyle dwStyle;    // WPSTYLE_* mentioned above
    }

    [Guid("F490EB00-1240-11D1-9888-006097DEACF9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IActiveDesktop
    {
        void ApplyChanges(AD_Apply dwFlags);
        void GetWallpaper([Out] StringBuilder pwszWallpaper, int cchWallpaper, int dwFlags);
        void SetWallpaper([In, MarshalAs(UnmanagedType.LPWStr)] string pwszWallpaper, int dwReserved);
        void GetWallpaperOptions(ref WALLPAPEROPT pwpo, int dwReserved);
        void SetWallpaperOptions([In] ref WALLPAPEROPT pwpo, int dwReserved);       
        void GetPattern([Out] StringBuilder pwszPattern, int cchPattern, int dwReserved);
        void SetPattern([In, MarshalAs(UnmanagedType.LPWStr)] string pwszPattern, int dwReserved);
        void GetDesktopItemOptions(IntPtr pco, int dwReserved);
        void SetDesktopItemOptions([In] IntPtr pco, int dwReserved);
        void AddDesktopItem([In] IntPtr pcomp, int dwReserved);
        void AddDesktopItemWithUI([In] IntPtr hwnd, [In] IntPtr pcomp, int dwReserved);
        void ModifyDesktopItem(IntPtr pcomp, int dwFlags);
        void RemoveDesktopItem([In] IntPtr pcomp, int dwReserved);
        void GetDesktopItemCount(IntPtr pcItems, int dwReserved);
        void GetDesktopItem(int nComponent, IntPtr pcomp, int dwReserved);
        void GetDesktopItemByID(IntPtr dwID, IntPtr pcomp, int dwReserved);
        void GenerateDesktopItemHtml([In, MarshalAs(UnmanagedType.LPWStr)] string pwszFileName, [In] IntPtr pcomp, int dwReserved);
        void AddUrl([In] IntPtr hwnd, [In, MarshalAs(UnmanagedType.LPWStr)] string pszSource, [In] IntPtr pcomp, int dwFlags);
        void GetDesktopItemBySource([In, MarshalAs(UnmanagedType.LPWStr)] string pwszSource, IntPtr pcomp, int dwReserved);
    }

    public class IActiveDesktopFactory
    {
        public static IActiveDesktop CreateInstance()
        {
            Type comType = Type.GetTypeFromCLSID(new Guid("75048700-EF1F-11D0-9888-006097DEACF9"));
            object comObj = Activator.CreateInstance(comType);
            return (IActiveDesktop)comObj;
        }
    }

    public static class IActiveDesktopExtensions
    {
        public static WallpaperStyle GetWallpaperStyle(this IActiveDesktop iad)
        {
            WALLPAPEROPT wpOpt = new WALLPAPEROPT();
            wpOpt.dwSize = Marshal.SizeOf(typeof(WALLPAPEROPT));
            iad.GetWallpaperOptions(ref wpOpt, 0);

            return wpOpt.dwStyle;
        }

        public static void SetWallpaperStyle(this IActiveDesktop iad, WallpaperStyle ws)
        {
            WALLPAPEROPT wpOpt = new WALLPAPEROPT();
            wpOpt.dwSize = Marshal.SizeOf(typeof(WALLPAPEROPT));
            wpOpt.dwStyle = ws;
            iad.SetWallpaperOptions(ref wpOpt, 0);
        }
    }
}
