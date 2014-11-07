using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Windows.Forms;

namespace WallpaperManager
{  
    class Program
    {
        static void Main(string[] args)
        {
            WallpaperChangerThread wct = new WallpaperChangerThread();
            wct.Start();

            using (var form = new frmTrayIcon())
            {
                form.WallpaperChanger = wct;

                form.Visible = false;
                form.WindowState = FormWindowState.Minimized;
                Application.Run(form);
            }

            wct.Stop();
        }
    }
}
