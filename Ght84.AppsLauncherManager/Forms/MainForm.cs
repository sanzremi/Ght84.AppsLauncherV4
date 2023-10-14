using Ght84.AppsLauncherManager.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ght84.AppsLauncherManager
{
    public partial class MainForm : Form
    {
        private Launcher _launcher = new Launcher();

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
            Opacity = 0;

            base.OnLoad(e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _launcher.Startup();
        }

        public void DisplayNotifyIconMessage( string message, string title ="Apps Launcher", ToolTipIcon toolTipIcon = ToolTipIcon.Info)
        {
            notifyIcon.BalloonTipIcon = toolTipIcon;
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipTitle = title;           
            notifyIcon.ShowBalloonTip(1000);
        }


    }
}
