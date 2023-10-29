using Ght84.AppsLauncherManager.Class;
using System;
using System.Windows.Forms;

// 1. Import the InteropServices type
using System.Runtime.InteropServices;
using Ght84.AppsLauncherManager.Helpers;

namespace Ght84.AppsLauncherManager
{
    public partial class MainForm : Form
    {
        //// 2. Import the RegisterHotKey Method
        //[DllImport("user32.dll")]
        //public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        //// 3. Import the UnregisterHotKey Method
        //[DllImport("user32.dll")]
        //public static extern bool UnregisterHotKey(IntPtr hWnd, int id);




        public KeyboardHook _hook = new KeyboardHook();

        private Launcher _launcher = new Launcher();
        private  ContextMenu _contextMenu = new ContextMenu();

        private int _uniqueHotkeyId = 1;
                    
        public MainForm()
        {
            InitializeComponent();

            // register the event that is fired after the key press.
            _hook.KeyPressed +=
                new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);     
            _hook.RegisterHotKey(0, Keys.F4);


        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {

            if (this.ShowInTaskbar == true)
            {
                HideForm();

            }
            else
            {
                ShowForm();
            }

        }


        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
            Opacity = 0;

            //chargement du menu contextuel du NotifyIcon
            createIconMenuStructure();


            base.OnLoad(e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 4. Register the "F9" hotkey
  
            //int HotKeyCode = (int)Keys.F4;
            //Boolean F9Registered = RegisterHotKey(
            //    this.Handle, _uniqueHotkeyId, 0x0000, HotKeyCode
            //);


            _launcher.Startup();
        }





        public void DisplayNotifyIconMessage( string message, string title ="Apps Launcher", ToolTipIcon toolTipIcon = ToolTipIcon.Info)
        {
            notifyIcon.BalloonTipIcon = toolTipIcon;
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipTitle = title;           
            notifyIcon.ShowBalloonTip(1000);
        }


  

        private void createIconMenuStructure()
        {
            // Add menu items to shortcut menu.  
            _contextMenu.MenuItems.Add("Afficher le menu", new System.EventHandler(this.menuItemShowForm_Click));

            _contextMenu.MenuItems.Add("Recharger la configuration", new System.EventHandler(this.menuItemReload_Click));
            _contextMenu.MenuItems.Add( "A propos...", new System.EventHandler(this.menuItemAbout_Click));
            notifyIcon.ContextMenu = _contextMenu;
        }

        private void menuItemAbout_Click(object sender, System.EventArgs e)
        {
            Forms.AboutForm aboutForm = new Forms.AboutForm();
            aboutForm.Show();
        }

        private void menuItemReload_Click(object sender, System.EventArgs e)
        {
            _launcher.ReloadConfigXmlFile(true);
        }

        private void menuItemShowForm_Click(object sender, System.EventArgs e)
        {
            ShowForm();
        }


        //protected override void WndProc(ref Message m)
        //{
        //    const int WM_HOTKEY = 0x0312;

        //    if (m.Msg == WM_HOTKEY)
        //    {
        //        int id = m.WParam.ToInt32();
        //        // MessageBox.Show(string.Format("Hotkey #{0} pressed", id));

        //        if (id == 1)
        //        {
        //            // This will never happen because we have registered the hotkey !
        //            // MessageBox.Show("F9 Was pressed !");

        //            if (this.ShowInTaskbar == true)
        //            {
        //                //HideForm();

        //            }
        //            else
        //            {
        //                ShowForm();
        //            }






        //        }
        //    }

        //    base.WndProc(ref m);
        //}

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 5. Unregister F9 HotKey
            HideForm();
            e.Cancel = true;    


            //Boolean F9UnRegistered = UnregisterHotKey(this.Handle, _uniqueHotkeyId);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            HideForm();
        }

        private void HideForm()
        {
            this.Visible = false; // Hide form window.
            this.ShowInTaskbar = false; // Remove from taskbar.
            this.Opacity = 0;
            this.TopMost = false;
        }

        private void ShowForm()
        {
            this.Visible = true; // Hide form window.
            this.ShowInTaskbar = true; // Remove from taskbar.
            this.Opacity = 1;
            this.TopMost = true;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            HideForm();
            _launcher.ExecuteCommand("STOP_RDP");
            
        }
    }
}
