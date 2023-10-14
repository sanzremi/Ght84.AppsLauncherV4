using Ght84.AppsLauncher.Server.Repositories;
using Ght84.AppsLauncherLibrary.Helpers;
using Ght84.AppsLauncherLibrary.InterProcessCommunication;
using Ght84.AppsLauncherLibrary.UdpCommunication;
using Ght84.AppsLauncherManager.Class;
using Ght84.AppsLauncherManager.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static Ght84.AppsLauncherLibrary.UdpCommunication.UdpReceiver;
using MessageBox = System.Windows.MessageBox;

namespace Ght84.AppsLauncherManager.Class
{
    internal class Launcher
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private ContextArgumentHelper _contextArgumentHelper = new ContextArgumentHelper();
        private AppsLauncherConfigurationRepository _repository = new AppsLauncherConfigurationRepository();

        private string _pipeName;
        private string _xmlFilePath;


        public void Startup()
        {
            try
            {
                //Nom du canal nommé (PipeName) dans le fichier de config app.setting
                _pipeName = ConfigurationHelper.DispatcherToManagerIpcPipeName; 

                // ETAPE 1 - Chargement de la configuration des appels contextuels
                _xmlFilePath = ConfigurationHelper.ConfigXmlFilePath;  // Nom et chemin du fichier de configuration XML
                _repository.LoadConfigXmlFile(_xmlFilePath);

                // ETAPE 2 - Intialisation de la communication Ipc (Cannaux Nommés) entre le service Windows AppsLauncherDispatcher et cet EXE AppsLauncherManager
                //           Se met en attente de la réception d'un message contenant l'appel contextuel (au format JSON)
                InitIpcServer();
            }

            catch (Exception ex)
            {
                _logger.Error($"Une erreur est survenue au démarrage de AppsLauncherManager : {ex.Message}");

                Program.MainForm.DisplayNotifyIconMessage($"Une erreur est survenue : {ex.Message}. Si le problème persiste, veuillez contacter le support informatique.", "AppsLauncher", toolTipIcon: ToolTipIcon.Error);
            }

        }


        /// <summary>
        /// Intialisation de la communication IPC entre le service AppsLauncherDispatcher et cet EXE AppsLauncherManager
        /// </summary>
        private async void InitIpcServer()
        {
            int currentSessionId;
            string pipeNameWithSessionId = string.Empty;

            try
            {
                // Id Session Windows courante
                currentSessionId = System.Diagnostics.Process.GetCurrentProcess().SessionId;
                pipeNameWithSessionId = $"{_pipeName}_{currentSessionId}";

                ////////// Initialisation du serveur IPC (Cannaux nommés)
                //////////IpcServer = new NamedPipeServer(pipeNameWithSessionId);
                //////////IpcServer.MessageReceived += IpcServer_MessageReceived;
                //////////await IpcServer.Start(); // En attente de réception

                IpcServer ipc = new IpcServer(pipeNameWithSessionId);
                ipc.MessageReceivedEventArgs += IpcServer_MessageReceived; // Lorsque des données seront reçues, alors l'évènemenent Ipc_OnNewMessageReceived sera déclenché
                ipc.StartReceiving();

                _logger.Info($"Réussite de l'initialisation du serveur IPC sur le cannal {pipeNameWithSessionId}. En attente de réception d'un message du service Windows AppsLauncherDispatcher.exe...");

            }
            catch (Exception ex)
            {
                throw new Exception($"Echec initialisation de la communication IPC sur le canal {pipeNameWithSessionId} : {ex.Message}");
            }


        }

        private void IpcServer_MessageReceived(object sender, string e)
        {
            string messageReceived = e;

            // Si le message reçu est vide ou null alors on quitte
            if (string.IsNullOrEmpty(messageReceived)) return;

            try
            {

                _logger.Info($"Message reçu du service Windows AppsLauncherDispatcher.exe : {messageReceived}");

                //// On vérifie si la date du fichier de config XML n'a pas changé. 
                //if (System.IO.File.GetLastWriteTime(AppliCHUMenuConfigXmlFilePathAndName) != AppliCHUMenuConfigXmlFileLastModifiedDate)
                //{
                //    //Si oui on le recharge
                //    ServiceProvider.GetService<ConfigMenusCommandesRepository>().LoadConfigXmlFile(AppliCHUMenuConfigXmlFilePathAndName);
                //    _logger.Info($"Rechargement du fichier {AppliCHUMenuConfigXmlFilePathAndName}");
                //}


                // Création d'un objet pour récupérer les données reçues 
                // Exemple messageReceived reçu (format JSON avec valeurs encodées en base64)
                // { "COMMAND":"Tk9URVBBRA==","ENV":"UFJE","WINDOWS_HOSTNAME":"RFQtMUxaVDZS","WINDOWS_USERNAME":"MDUxMjg0","WINDOWS_USERDOMAIN":"QURDSEE=","WINDOWS_USERDNSDOMAIN":"QURDSEEuTE9DQUw="}
                _contextArgumentHelper.SetFromJsonMessage(jsonMessage: messageReceived, decodeBase64AllValues: true);
                

                // Récupération du code de la commande à lancer dans le message 
                string commandCode = _contextArgumentHelper.ContextArguments.GetValue("COMMAND");


                if (!String.IsNullOrEmpty(commandCode))
                {
                    // On récupère l'objet commande qui est chargé en mémoire (chargement à partir d'un fichier XML de config lors de l'initialisation de cet exe AppsLauncherManager)
                    Command command = _repository.GetCommandByCode(commandCode, out _);

                    if (command != null) // Si l'objet a été trouvé 
                    {

                        Program.MainForm.DisplayNotifyIconMessage($"Lancement de la commande '{commandCode}'");

                        // On lance reellement l'appel contextuel défini dans la commande (Process.start ...)
                        new CommandProcessor().ExecuteCommand(command, _contextArgumentHelper); 

                    }
                    else // l'objet n'existe pas
                    {
                        throw new Exception($"La COMMANDE avec le code '{commandCode}' n'existe pas. Vérifier la syntaxe d'appel de AppsLauncherWrapper.exe ou le paramétrage dans le fichier de configuration XML des appels contextuels");
                    }

                }
                else
                {
                    throw new Exception($"La COMMANDE est absente. Vérifier la syntaxe d'appel de AppsLauncherWrapper.exe ou le paramétrage dans le fichier XML des appels contextuels");

                }


            }
            catch (Exception ex)
            {
                _logger.Error($"{ex.Message}");

                Program.MainForm.DisplayNotifyIconMessage($"Une erreur est survenue : {ex.Message}. Si le problème persiste, veuillez contacter le support informatique.", "AppsLauncher", toolTipIcon: ToolTipIcon.Error);
            }

        }

 

    }




}
