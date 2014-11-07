using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace WallpaperManager
{
    class Monitor
    {
        public Rectangle Rect { get; set; }
        public Rectangle WorkRect { get; set; }
        public bool Primary { get; set; }
        public string DeviceName { get; set; }
    }

    enum WallpaperMode
    {
        Random,
        FairRandom,
        Sequential,
        Single
    }
    class FileOrDirectory
    {
        public bool IsDirectory { get; set; }
        public string Path { get; set; }
        public bool IsRecursive { get; set; }
    }
    class MonitorSettings
    {
        public Color BackgroundColor { get; set; }
        public int ChangeInterval { get; set; }
        public WallpaperMode Mode { get; set; }
        public WallpaperDisposition Disposition { get; set; }
        public string SingleImage { get; set; }
        public FileOrDirectory[] RandomSources { get; set; }
    }

    class MonitorState
    {
        public DateTime LastChange { get; set; }
        public string CurrentWallpaper { get; set; }
    }

    enum WallpaperDisposition
    {
        Fit,
        Fill,
        Tile,
        Stretch,
        Center,
        MatchWidth,
        MatchHeight
    }
    class WallpaperGenSetttings
    {
        public Image Image { get; set; }
        public WallpaperDisposition Disposition { get; set; }
        public Color BackgroundColor { get; set; }
    }
}
