using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Ght84.AppsLauncherLibrary.UdpCommunication
{
    public class UdpSender
    {

        private IPEndPoint _ipEndPoint;

        public UdpSender(string ip, int port)
        {
            IPAddress iPAddress =  ParseIPAddress(ip);
          
            _ipEndPoint = new IPEndPoint(iPAddress, port);
        }

        public void  Send(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            using (UdpClient udpClient = new UdpClient()) 
            {                                  
                var data = Encoding.UTF8.GetBytes(message);

                 udpClient.Send(data, data.Length, _ipEndPoint);
            }

        }

        private IPAddress ParseIPAddress(string ipAddressText)
        {
            if (string.IsNullOrEmpty(ipAddressText) ||
                IPAddress.TryParse(ipAddressText, out var ipAddress) == false)
            {
                return null;
            }
            return ipAddress;
        }


    }
}
