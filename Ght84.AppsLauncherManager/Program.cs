using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

// v4
namespace Ght84.AppsLauncherManager
{
    internal static class Program
    {

        public  static MainForm MainForm;
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // Si cette application est déjà lancée dans la même session, on ne la lance qu'une fois
            if (ApplicationIsAlreadyRunning())
            {
                //on quitte
                return;
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (MainForm mainForm = new MainForm())
            {
                MainForm = mainForm;
                Application.Run(mainForm);                
            }

        }


        private static bool ApplicationIsAlreadyRunning()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);

            // test if there's another process running in current session.
            var intTotalRunningInCurrentSession = processes.Count(prc => prc.SessionId == currentProcess.SessionId);

            return intTotalRunningInCurrentSession > 1;
        }


    }





}
