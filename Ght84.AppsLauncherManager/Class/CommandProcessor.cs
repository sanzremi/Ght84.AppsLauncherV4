
using Ght84.AppsLauncherLibrary.Helpers;
using Ght84.AppsLauncherManager.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Ght84.AppsLauncherManager.Class
{ 
    public class CommandProcessor
    {

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        
        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        const int SW_RESTORE = 9;



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

        //Dictionary<string, string> args
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

            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    //WindowStyle = ProcessWindowStyle.Maximized,
                    FileName = targetPath,
                    Arguments = arguments.ToString(),
                    UseShellExecute = true
                };

                p.Start();

                // on force la fenetre au premier plan
                IntPtr handle = p.MainWindowHandle;
                if (IsIconic(handle)) ShowWindow(handle, SW_RESTORE);
                SetForegroundWindow(handle);

            }

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
