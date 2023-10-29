
using Ght84.AppsLauncherLibrary.Helpers;
using Ght84.AppsLauncherManager.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using MessageBox = System.Windows.MessageBox;

namespace Ght84.AppsLauncherManager.Class
{
    public class CommandProcessor
    {
         //Valeur par défaut du timeout (en ms) pour attendre le handle de la fenetre principale de la commande
         const int DEFAULT_TIMEOUT_MAINWINDOWS = 1000;

        //[DllImport("user32.dll")]
        //public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);


        //[DllImport("user32.dll")]
        //private static extern bool SetForegroundWindow(IntPtr hWnd);

        //[DllImport("User32.dll")]
        //private static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        //[DllImport("User32.dll")]
        //private static extern bool IsIconic(IntPtr handle);

        //const int SW_RESTORE = 9;

        //[DllImport("user32.dll")]
        //internal static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);
        //const int WM_SYSCOMMAND = 0x0112;


        //[DllImport("user32.dll")]
        //private static extern bool AllowSetForegroundWindow(IntPtr hWnd);

        //[DllImport("user32.dll")]
        //static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        //static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);


        //const UInt32 SWP_NOSIZE = 0x0001;
        //const UInt32 SWP_NOMOVE = 0x0002;
        //const UInt32 SWP_SHOWWINDOW = 0x0040;


        static List<string> paths;
        static WindowsIdentity currentUser = WindowsIdentity.GetCurrent();

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public CommandProcessor()
        {
            if (paths == null)
            {
                paths = Environment.GetEnvironmentVariable("PATH").ToLower().Split(';').ToList();
            }

        }


        public void ExecuteCommand(Command command, ContextArgumentHelper contextArgumentHelper)
        {

            // verification des droits
            if (command.ADGroups != "")
                if (!UserIsInRole(command.ADGroups))
                {
                    MessageBox.Show("Vous n'avez pas les droits necessaires", "Avertissement");
                    return;
                }

            // verification si il y a un message a afficher
            if (!string.IsNullOrEmpty(command.InformationMessage))
                MessageBox.Show(command.InformationMessage, "Information");
            
            if (!string.IsNullOrEmpty(command.WarningMessage))
            {
                MessageBox.Show(command.WarningMessage, "Avertissement");
                return;
            }

            // verification si le fichier existe
            string targetPath = string.Empty;

            // On teste la première cible
            targetPath = command.Executable.TargetPath;
            if (!System.IO.File.Exists(command.Executable.TargetPath))
                if (!EnvironmentFileExist(command.Executable.TargetPath))
                {
                    // On teste la deuxième cible
                    if (!string.IsNullOrEmpty(command.Executable.AlternativeTargetPath))
                    {
                        targetPath = command.Executable.AlternativeTargetPath;
                        if (!System.IO.File.Exists(command.Executable.AlternativeTargetPath))
                            if (!EnvironmentFileExist(command.Executable.AlternativeTargetPath))
                            {
                                return;
                            }
                    }


                    return;
                }

            if (String.IsNullOrEmpty(targetPath) ) 
            {
                throw new Exception($"L'exécutable cible {command.Executable.TargetPath} ou {command.Executable.AlternativeTargetPath} n'existe pas sur l'ordinateur local");
            }



            StringBuilder arguments = new StringBuilder(command.Arguments);
            if (arguments.Length > 0)
                foreach (var ca in contextArgumentHelper.ContextArguments)
                {
                    arguments.Replace($"%{ca.Key.ToUpper()}%", ca.Value.Value);
                }



            _logger.Debug($"Début de lancement de la commande {command.Code} : {targetPath} {arguments.ToString()}");

            IntPtr handleMainWindows;
            string titleMainWindows = string.Empty;

            long  startTicks = DateTime.Now.Ticks;   

            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Normal,

                    ////WindowStyle = ProcessWindowStyle.Maximized,
                    FileName = targetPath,
                    Arguments = arguments.ToString(),
                    UseShellExecute = true
                };

                // Démarrage du process
                p.Start();

                // Récupération du délai à attendre avant de lancer la commande AutoIt WinActivate
                int TimeoutMainWindow = (command.Executable.TimeoutMainWindow == null ? DEFAULT_TIMEOUT_MAINWINDOWS : (int)command.Executable.TimeoutMainWindow);

                // On attend que le handle de la fenetre principale du process soit disponible
                while (p.MainWindowHandle == IntPtr.Zero && TimeoutMainWindow > 0)
                {
                    p.Refresh();
                    System.Threading.Thread.Sleep(250); //Wait 0.25 seconds.
                    TimeoutMainWindow -= 250;
                }

                if (p.MainWindowHandle == IntPtr.Zero)
                {
                    _logger.Error($"Expiration du timeout de {(TimeoutMainWindow / 10).ToString()} seconde(s) pour l'ouverture de la fenêtre Windows associée à la commande");
                    throw new Exception($"La fenêtre Windows associée à la commande {command.Code} n'a pas pu se lancer avant expiration du timeout de {(TimeoutMainWindow / 10).ToString()} seconde(s)");
                }
                else
                {
                    // Récupération du Handle de la fenetre 
                    handleMainWindows = p.MainWindowHandle;
                    titleMainWindows = p.MainWindowTitle;
                }



                //_logger.Debug($"Attente délai de {WaitDelayBeforeWinActivate.ToString()} ms avant d'activer la fenêtre de l'éxécutable");

                // Pause de x ms le temps que le process démarre
                //System.Threading.Thread.Sleep((int)WaitDelayBeforeWinActivate);

                // Récupération du Handle de la fenetre 
                //handleMainWindows = p.MainWindowHandle;
                //titleMainWindows = p.MainWindowTitle;

                long endTicks = DateTime.Now.Ticks;
                long delayToStart = (endTicks - startTicks)/10000; //10,000 Ticks is a millisecond


                _logger.Debug($"Handle et titre de la fenetre Windows liée à la commande : {handleMainWindows.ToString()} - {titleMainWindows}");
                _logger.Debug($"Commande lancée en {delayToStart.ToString()} ms");

            }

            // on force la fenetre au premier plan avec les fonctions magiques AutoIt ;-)))
            AutoIt.AutoItX.WinActivate(handleMainWindows);



            _logger.Debug($"Fin de lancement de la commande {command.Code}");

        }



        public bool EnvironmentFileExist(string file)
        {
            bool result = false;
            foreach (var path in paths)
                if (System.IO.File.Exists(System.IO.Path.Combine(path, file)))
                {
                    result = true;
                    break;
                }
            return result;
        }

        public static bool UserIsInRole(string role)
        {
            WindowsPrincipal principal = new WindowsPrincipal(currentUser);
            return principal.IsInRole(role);
        }


    }
}
