using System;
using System.Configuration;

namespace Ght84.AppsLauncherLibrary.Helpers
{
    public static class ConfigurationHelper
    {
        public static string ConfigXmlFilePath 
        { 
            get
            {
                string value = ConfigurationManager.AppSettings.Get("AppsLauncher_ConfigXmlFilePath");
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Le paramètre 'AppsLauncher_ConfigXmlFilePath' est absent ou vide dans le fichier de configuration app.config");
                }                
                return value;
            }
        }


        public static string CodeEnv
        {
            get
            {
                string value = ConfigurationManager.AppSettings.Get("AppsLauncher_CodeEnv");
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Le paramètre 'AppsLauncher_CodeEnv' est absent ou vide dans le fichier de configuration app.config");
                }
                return value;
            }
        }

        public static int WrapperToDispatcherUdpPort
        {
            get
            {
                int port;
                string value = ConfigurationManager.AppSettings.Get("AppsLauncher_WrapperToDispatcherUdpPort");
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Le paramètre 'AppsLauncher_WrapperToDispatcherUdpPort' est absent ou vide dans le fichier de configuration app.config");
                }
                if (!int.TryParse(value,out port))
                {
                    throw new Exception($"La valeur '{value}' du paramètre 'AppsLauncher_WrapperToDispatcherUdpPort' est incorrecte dans le fichier de configuration app.config");
                }

                return port;
            }
        }

        public static string DispatcherToManagerIpcPipeName
        {
            get
            {
                string pipeName = ConfigurationManager.AppSettings.Get("AppsLauncher_DispatcherToManagerIpcPipeName");
                if (string.IsNullOrEmpty(pipeName))
                {
                    throw new Exception("Le paramètre 'AppsLauncher_DispatcherToManagerIpcPipeName' est absent ou vide dans le fichier de configuration app.config");
                }

                return pipeName;
            }
        }

        public static string WrapperUnderRDSWindowsToCloseClassName
        {
            get
            {
                string className = ConfigurationManager.AppSettings.Get("AppsLauncher_WrapperUnderRDS_WindowsToCloseClassName");
                if (string.IsNullOrEmpty(className))
                {
                    throw new Exception("Le paramètre 'AppsLauncher_WrapperUnderRDS_WindowsToCloseClassName' est absent ou vide dans le fichier de configuration app.config");
                }

                return className;
            }
        }

        public static string WrapperUnderRDSWindowsToCloseTitle
        {
            get
            {
                string title = ConfigurationManager.AppSettings.Get("AppsLauncher_WrapperUnderRDS_WindowsToCloseTitle");
                if (string.IsNullOrEmpty(title))
                {
                    throw new Exception("Le paramètre 'AppsLauncher_WrapperUnderRDS_WindowsToCloseTitle' est absent ou vide dans le fichier de configuration app.config");
                }

                return title;
            }
        }


    }
}