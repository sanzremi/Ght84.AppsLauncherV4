using Microsoft.Win32;

namespace Ght84.AppsLauncherLibrary.Helpers
{
    public static class RegistryHelper
    {

        /// <summary>
        /// Valeur de la clé de registre Ordinateur\HKEY_LOCAL_MACHINE\SOFTWARE\Ght84\Ght84.AppsLauncher
        /// Permettant de savoir si l'appel contextuel (AppsLauncherWrapper executé depuis une session RSD) être exécuté 
        /// localement (sur la session RDS) ou à distance (sur le PC client hote qui a démarré la session RDS) 
        /// </summary>
        /// <returns>true ou false</returns>
        public static bool GetFlagRedirectCall()
        {
            RegistryKey key;
            string RedirectCall = string.Empty;


            var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

            key = localMachine.OpenSubKey("SOFTWARE\\Ght84\\Ght84.AppsLauncher");
            if (key != null)
            {
                RedirectCall = key.GetValue("RedirectCall").ToString();
            }
            // on regarde éventuelle en 32 bits
            else
            {
                localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                if (key != null)
                {
                    RedirectCall = key.GetValue("RedirectCall").ToString();
                }
            }

            if (string.IsNullOrEmpty(RedirectCall) || RedirectCall == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        public static SessionInfo GetSessionInfo(int sessionId)
        {
            RegistryKey key;
            string sessionName = string.Empty;
            string userName = string.Empty; 
            string userDomain = string.Empty; 
            string userDnsDomain = string.Empty; ;
            string tsClientName = string.Empty;

            key = Registry.CurrentUser.OpenSubKey("Volatile Environment\\" + sessionId.ToString());
            if (key != null)
            {
                tsClientName = key.GetValue("CLIENTNAME").ToString();
                sessionName = key.GetValue("SESSIONNAME").ToString();
            }

            key = Registry.CurrentUser.OpenSubKey("Volatile Environment");
            if (key != null)
            {
                userName = key.GetValue("USERNAME").ToString();
                userDomain = key.GetValue("USERDOMAIN").ToString();
                userDnsDomain = key.GetValue("USERDNSDOMAIN").ToString();
            }

            return new SessionInfo(sessionId, sessionName, userName, userDomain, userDnsDomain, tsClientName);   

        }

        public struct SessionInfo
        {
            public int SessionId;
            public string SessionName;
            public string UserName;
            public string UserDomain;
            public string UserDnsDomain;
            public string TsClientName;


            public SessionInfo(int sessionId,  string sessionName = "", string userName = "", string userDomain = "", string userDnsDomain = "", string tsClientName = "")
            {
                SessionId = sessionId;
                SessionName = sessionName;
                UserName = userName;
                UserDomain = userDomain;
                UserDnsDomain = userDnsDomain;
                TsClientName = tsClientName;

            }

        }



    }
}
