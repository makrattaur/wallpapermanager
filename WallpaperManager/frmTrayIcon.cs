using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WallpaperManager
{
    public partial class frmTrayIcon : Form
    {
        public WallpaperChangerThread WallpaperChanger { get; set; }

        public frmTrayIcon()
        {
            InitializeComponent();
        }

        private void tsmiQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsmiSkip_Click(object sender, EventArgs e)
        {
            WallpaperChanger.SkipCycle();
        }

        bool isPaused = false;
        private void tsmiPause_Click(object sender, EventArgs e)
        {
            if (!isPaused)
            {
                WallpaperChanger.PauseCycle();
                tsmiPause.Text = "Resume rotation";
                isPaused = true;
            }
            else
            {
                WallpaperChanger.ResumeCycle();
                tsmiPause.Text = "Pause rotation";
                isPaused = false;
            }
        }

        private void tsmiReloadSettings_Click(object sender, EventArgs e)
        {
            WallpaperChanger.ReloadConfig();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80;  // Turn on WS_EX_TOOLWINDOW
                return cp;
            }
        }

        private void frmTrayIcon_Load(object sender, EventArgs e)
        {
            Hide();
            ShowInTaskbar = false;
            SetVisibleCore(false);
        }
    }
}
