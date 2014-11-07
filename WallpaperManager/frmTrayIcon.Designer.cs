namespace WallpaperManager
{
    partial class frmTrayIcon
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTrayIcon));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsTrayIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiReloadSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiPause = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSkip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsTrayIcon.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.cmsTrayIcon;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Wallpaper Manager";
            this.notifyIcon.Visible = true;
            // 
            // cmsTrayIcon
            // 
            this.cmsTrayIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSettings,
            this.tsmiReloadSettings,
            this.toolStripSeparator1,
            this.tsmiPause,
            this.tsmiSkip,
            this.toolStripSeparator2,
            this.tsmiQuit});
            this.cmsTrayIcon.Name = "cmsTrayIcon";
            this.cmsTrayIcon.ShowImageMargin = false;
            this.cmsTrayIcon.Size = new System.Drawing.Size(130, 126);
            // 
            // tsmiSettings
            // 
            this.tsmiSettings.Name = "tsmiSettings";
            this.tsmiSettings.Size = new System.Drawing.Size(129, 22);
            this.tsmiSettings.Text = "Settings...";
            // 
            // tsmiReloadSettings
            // 
            this.tsmiReloadSettings.Name = "tsmiReloadSettings";
            this.tsmiReloadSettings.Size = new System.Drawing.Size(129, 22);
            this.tsmiReloadSettings.Text = "Reload settings";
            this.tsmiReloadSettings.Click += new System.EventHandler(this.tsmiReloadSettings_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(126, 6);
            // 
            // tsmiPause
            // 
            this.tsmiPause.Name = "tsmiPause";
            this.tsmiPause.Size = new System.Drawing.Size(129, 22);
            this.tsmiPause.Text = "Pause rotation";
            this.tsmiPause.Click += new System.EventHandler(this.tsmiPause_Click);
            // 
            // tsmiSkip
            // 
            this.tsmiSkip.Name = "tsmiSkip";
            this.tsmiSkip.Size = new System.Drawing.Size(129, 22);
            this.tsmiSkip.Text = "Skip rotation";
            this.tsmiSkip.Click += new System.EventHandler(this.tsmiSkip_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(126, 6);
            // 
            // tsmiQuit
            // 
            this.tsmiQuit.Name = "tsmiQuit";
            this.tsmiQuit.Size = new System.Drawing.Size(129, 22);
            this.tsmiQuit.Text = "Quit";
            this.tsmiQuit.Click += new System.EventHandler(this.tsmiQuit_Click);
            // 
            // frmTrayIcon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "frmTrayIcon";
            this.ShowInTaskbar = false;
            this.Text = "frmTrayIcon";
            this.Load += new System.EventHandler(this.frmTrayIcon_Load);
            this.cmsTrayIcon.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip cmsTrayIcon;
        private System.Windows.Forms.ToolStripMenuItem tsmiSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiPause;
        private System.Windows.Forms.ToolStripMenuItem tsmiSkip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuit;
        private System.Windows.Forms.ToolStripMenuItem tsmiReloadSettings;
    }
}