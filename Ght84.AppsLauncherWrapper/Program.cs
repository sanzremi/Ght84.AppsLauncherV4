using Ght84.AppsLauncherLibrary.Helpers;
using Ght84.AppsLauncherLibrary.UdpCommunication;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using static Ght84.AppsLauncherLibrary.Helpers.RegistryHelper;

namespace Ght84.AppsLauncherWrapper
{
    class Program
    {
        [DllImport("user32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_CLOSE = 0xF060;



        private static string _codeEnv;
        private static bool _redirectCall = false;
        private static SessionInfo _sessionInfo;

        private static Logger _logger = LogManager.GetCurrentClassLogger();


        static void Main(string[] args)
        {
            try
            {
                _logger.Info($"Démarrage de Ght84.AppsLauncherWrapper.exe");


                /// Valeur de la clé de registre Ordinateur\HKEY_LOCAL_MACHINE\SOFTWARE\Ght84\Ght84.AppsLauncher
                /// Permettant de savoir si l'appel contextuel (AppsLauncherWrapper executé depuis une session RSD) être exécuté 
                /// localement (sur la session RDS) ou à distance (sur le PC client hote qui a démarré la session RDS) 
                _redirectCall = RegistryHelper.GetFlagRedirectCall();

                // Port UDP pour joindre par socket le service Ght84.AppsLauncherDispatcher
                int _udpPort = ConfigurationHelper.WrapperToDispatcherUdpPort;

                // Code de l'environnement des appels contextuels
                //   PRD  =  Sélection des appels contextuels "PRODUCTION" dans le fichier de configuration XML
                //   TST  =  Sélection des appels contextuels "TEST" dans le fichier de configuration XML 
                _codeEnv = ConfigurationHelper.CodeEnv;

                
                // Récupération dans la clé de registre \HKEY_CURRENT_USER\Volatile Environment des informations sur la session Windows courante (Session et UserName) 
                // qui exécute ce programme Ght84.AppsLauncherWrapper.exe
                // La session peut être soit locale ou soit à distance en session RDS  (exemple serveur RDS EASILY)
                // Id Session Windows courante
                int currentSessionId = System.Diagnostics.Process.GetCurrentProcess().SessionId; // N° de session Windows courante et active            
                _sessionInfo = RegistryHelper.GetSessionInfo(currentSessionId);


                //Récupération de l'adresse IP du poste client qui héberge le service Ght84.AppsLauncherDispatcher
                // (pour réaliser une communication UDP entre AppsLauncherWrapper vers AppsLauncherDispatcher)
                IpAndHost ipAndHost = GetHostIpAdress();
                if (ipAndHost.IsEmpty()) throw new Exception("L'adresse IP du poste client qui héberge le service Ght84.AppsLauncherDispatcher n'a pas été trouvé");


                // On ouvre une connexion UDP vers ce poste client qui héberge le service Windows Ght84.AppsLauncherDispatcher
                UdpSender udpSender = new UdpSender(ipAndHost.Ip, _udpPort);
                _logger.Debug($"Connexion socket UDP vers le service Ght84.AppsLauncherDispatcher ip={ipAndHost.Ip}, host={ipAndHost.Host}, port={_udpPort.ToString()}");

                //Récupération des arguments 'args' de la ligne de commande passée lors de l'appel de Ght84.AppsLauncherWrapper.exe 
                //Exemple 1 (si appel direct) : Ght84.AppsLauncherWrapper.exe COMMAND:=PHARMA_PRESC|EPISODE:=622009830|US:=3508
                //Exemple 2 (si appel par Url Protocol APPSLAUNCHER) : Ght84.AppsLauncherWrapper.exe APPSLAUNCHER:COMMAND:=PHARMA_PRESC|EPISODE:=622009830|US:=3508
                string stringOfArgs = args[0];

                // puis conversion en format Json (valeurs encodées en base64)
                // Exemple message  encodé (format JSON avec valeurs encodées en base64)
                // { "COMMAND":"Tk9URVBBRA==","ENV":"UFJE","WINDOWS_HOSTNAME":"RFQtMUxaVDZS","WINDOWS_USERNAME":"MDUxMjg0","WINDOWS_USERDOMAIN":"QURDSEE=","WINDOWS_USERDNSDOMAIN":"QURDSEEuTE9DQUw="}
                string jsonMessage = TransformStringOfArgsToJsonMessage(stringOfArgs);
                _logger.Debug($"Transformation des arguments de la ligne de commande '{stringOfArgs}' en message json (valeurs encodées en base64) '{jsonMessage}'");

                // Le message est envoyée au service Windows Ght84.AppsLauncherDispatcher par socket UDP (quilui même le renverra au AppsLauncherManager de la session Windows Active
                udpSender.Send(jsonMessage);

                _logger.Debug($"Envoi du message sur le socket UDP : {jsonMessage}");

                _logger.Info($"Envoi réussi du message '{jsonMessage}' par socket UDP au service Ght84.AppsLauncherDispatcher : ip {ipAndHost.Ip}, host {ipAndHost.Host}, port {_udpPort.ToString()}");

            }
            catch (Exception ex)
            {
                _logger.Error($"Echec d'envoi du message par socket UDP au service Ght84.AppsLauncherDispatcher : {ex.ToString()}");
            }

        }

        private static string TransformStringOfArgsToJsonMessage(string stringOfArgs)
        {
            string jsonMessage = String.Empty;
                  

            List<string> newArgs = new List<string>();
            // Si appel par Url Protocol APPSLAUNCHER
            // Alors on retire le mot clé APPSLAUNCHER et on décode l'URL d'appel
            if (stringOfArgs.ToUpper().StartsWith("APPSLAUNCHER:"))
            {
                stringOfArgs = stringOfArgs.Substring(("APPSLAUNCHER:").Length);
                stringOfArgs = WebUtility.UrlDecode(stringOfArgs);
            }

            // Construction d'une liste contenant les différents paramètres passés en arguments 
            // Exemple COMMAND, ENV, MATRICULE, IPP, US, ...
            ContextArgumentHelper contextArgumentHelper = new ContextArgumentHelper();
            contextArgumentHelper.SetFromStringOfArgs(stringOfArgs: stringOfArgs, argSeparator: '|', codeValueSeparator: ":=");

           
            // Rajout du code environnement (PRD ou TST) dans la liste
            contextArgumentHelper.ContextArguments.Add("ENV", _codeEnv);
           
            // Rajout du HostName dans la liste
            contextArgumentHelper.ContextArguments.Add("WINDOWS_HOSTNAME", Dns.GetHostName());
           
            // Rajout du WINDOWS_USERNAME (User AD, exemple 051284)
            contextArgumentHelper.ContextArguments.Add("WINDOWS_USERNAME", _sessionInfo.UserName);
           
            // Rajout du WINDOWS_USERDOMAIN (Domaine AD, exemple adcha)
            contextArgumentHelper.ContextArguments.Add("WINDOWS_USERDOMAIN", _sessionInfo.UserDomain);
         
            // Rajout du WINDOWS_USERDNSDOMAIN (Domaine AD complet, exemple adcha.local)
            contextArgumentHelper.ContextArguments.Add("WINDOWS_USERDNSDOMAIN", _sessionInfo.UserDnsDomain);
           

            //On convertit en Json et on encode les valeurs en base64
            jsonMessage = contextArgumentHelper.ConvertToJsonMessage(encodeBase64AllValues: true);

            return jsonMessage;

        }


        private static IpAndHost GetHostIpAdress()
        {
            IpAndHost ipAndHost = new IpAndHost();

            // Id Session Windows courante   
            _logger.Debug($"Id Session Windows courante {_sessionInfo.SessionId.ToString()}");

            // Nom du hostName courant
            string host = Dns.GetHostName();
            _logger.Debug($"HostName courant {host}");

            // Dans le cadre d'une exécution de l'appel de cette application Ght84.AppsLauncherClient.exe dans une session distante RDP (TS)
            // On récupère via une interrogation à la base de registre HKCU\Volatile Environment le HostName du PC client
            string TsClientName = _sessionInfo.TsClientName;
            _logger.Debug($"TsClientName récupéré dans la clé de registre HKCU\\Volatile Environment {TsClientName}");
            _logger.Debug($"Paramètre RedirectCall {_redirectCall.ToString()}");

            // Cas N°1 : Exécution de Ght84.AppsLauncherWrapper.exe dans une session distante RDP et l'exécution à Distance des appels contextuels est autorisée depuis ce serveur RDP
            if (_redirectCall  && !String.IsNullOrEmpty(TsClientName))
            {
                _logger.Debug($"Cas N°1 : Exécution de Ght84.AppsLauncherWrapper.exe dans une session distante RDP et l'exécution à distance des appels contextuels est autorisée depuis ce serveur RDP");
                try
                {
                    // On récupère la liste des adresses ip du client Ts
                    string ipV4Adress = GetIpV4Adress(TsClientName);
                    ipAndHost = new IpAndHost(ipV4Adress, host);

                }
                catch (Exception ex)
                {
                    _logger.Error($"Erreur lors de la recherche ip et host du poste client qui héberge le service Windows Ght84.AppsLauncherDispatcher: {ex.ToString()}");
                }
            }
            // Cas N°2 : Exécution Ght84.AppsLauncherWrapper.exe et des appels contextuels sur la même machine
            else
            {
                _logger.Debug($"Cas N°2 : Exécution Ght84.AppsLauncherWrapper.exe et des appels contextuels sur le même poste client");

                try
                {
                    string ipV4Adress = GetIpV4Adress(host);
                    ipAndHost = new IpAndHost(ipV4Adress, host);

                }
                catch (Exception ex)
                {
                    _logger.Error($"Erreur lors de la recherche ip et host du poste client qui héberge le service Windows Ght84.AppsLauncherDispatcher: {ex.ToString()}");
                }

            }

            return ipAndHost;

        }

        private static string GetIpV4Adress(string host)
        {
            string ipv4Address = String.Empty;

            foreach (IPAddress currrentIPAddress in Dns.GetHostAddresses(host))
            {
                if (currrentIPAddress.AddressFamily.ToString() == System.Net.Sockets.AddressFamily.InterNetwork.ToString() && !IPAddress.IsLoopback(currrentIPAddress))
                {
                    ipv4Address = currrentIPAddress.ToString();
                    break;
                }
            }

            return ipv4Address;
        }


        private struct IpAndHost
        {
            public string Ip;
            public string Host;

            public IpAndHost(string ip = "", string host = "")
            {
                Ip = ip;
                Host = host;
            }

            public bool IsEmpty()
            {
                return (String.IsNullOrEmpty(Ip));
            }

        }

        private void CloseWindows()
        {
            // Retrieve the handler of the window
            int iHandle = FindWindow("Notepad", "Untitled - Notepad");
            if (iHandle > 0)
            {
                // Close the window using API
                SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
        }
    }








}
}
