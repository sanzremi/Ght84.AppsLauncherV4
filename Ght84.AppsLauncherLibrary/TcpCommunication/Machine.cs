using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherLibrary.TcpCommunication
{
    public class Machine
    {
        private static Object _classLocker = new Object();
        private static Machine _machine;

        private Machine()
        {
        } // end private Machine()

        public static Machine GetInstance()
        {
            if (_machine == null)
            {
                lock (_classLocker)
                {
                    if (_machine == null)
                    {
                        _machine = new Machine();
                    }
                }
            }
            return _machine;
        } // end public static Machine getInstance()

        public String GetUsername()
        {
            string username = null;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
                ManagementObjectCollection collection = searcher.Get();
                username = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];

                // Remove the domain part from the username
                string[] usernameParts = username.Split('\\');
                // The username is contained in the last string portion.
                username = usernameParts[usernameParts.Length - 1];
            }
            catch (Exception)
            {
                // The system currently has no users who are logged on
                // Set the username to "SYSTEM" to denote that
                username = "SYSTEM";
            }
            return username;
        } // end String getUsername()
    } // end class Machine
}
