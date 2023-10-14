using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Ght84.AppsLauncherLibrary.UdpCommunication
{
    public class UdpReceiver
    {
        public delegate void MessageReceived(object sender, UdpReceivedMessageArgs args);

        public event MessageReceived OnMessageReceived;

        private IPEndPoint _ipEndPoint;

        private bool _running;

        private int _port;
        private UdpClient _udpClient;


        private Thread _connectionThread;

        public UdpReceiver(int port)
        {
            _port = port;
            
            // To accept access from other machines, I have to set "IPAddress.Any".
            _ipEndPoint = new IPEndPoint(IPAddress.Any, port);

            _udpClient = new UdpClient(_ipEndPoint);

            this._running = false;
  
        } 


        public void Start()
        {
            if (!_running)
            {

                this._running = true;
                this._connectionThread = new Thread
                    (new ThreadStart(ListenForClientConnections));
                this._connectionThread.Start();
            } 

        } 


        public void Stop()
        {
            if (this._running)
            {
                this._udpClient.Close();
                this._running = false;
            }
        } 

        public bool Running()
        {
            return this._running;
        } 


        public void ListenForClientConnections()
        {
            while (this._running)
            {
                Byte[] receiveBytes = new byte[0];


                try 
                { 
                    receiveBytes = _udpClient.Receive(ref _ipEndPoint);
                }
                catch (Exception err)
                {
                    
                }
                lock(this)
                {
                    string message = Encoding.UTF8.GetString(receiveBytes, 0, receiveBytes.Length);
                    RaiseUdpMessageReceived(new UdpReceivedMessageArgs(_ipEndPoint.Address, _ipEndPoint.Port, message));
                }
            }


    }



        private void RaiseUdpMessageReceived(UdpReceivedMessageArgs args)
        {
            if (OnMessageReceived != null)
            {
                OnMessageReceived(this, args);
            }
        }


    }
}
