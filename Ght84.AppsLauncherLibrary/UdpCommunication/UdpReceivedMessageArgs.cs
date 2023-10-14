using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherLibrary.UdpCommunication
{
    public class UdpReceivedMessageArgs : EventArgs
    {
        public IPAddress IPAdress { get; set; }
        public int Port { get; set; }
        public string Message { get; set; }

        public UdpReceivedMessageArgs(IPAddress iPAdress, int port, string message)
        {
            this.IPAdress = iPAdress;
            this.Port = port;
            this.Message = message;
        }

    }

}
