using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Ght84.AppsLauncherLibrary.TcpCommunication;
using Ght84.AppsLauncherLibrary.UdpCommunication;

public delegate void MessageReceivedEventHandler(object sender, string args);


namespace Ght84.AppsLauncherLibrary.TcpCommunication
{
    public class MessageClient
    {
        private int _port;
        private TcpClient _tcpClient;
        private BinaryFormatter _bFormatter;
        private Thread _listenThread;
        private bool _running, _disposed;
        public event MessageReceivedEventHandler MessageReceived;


        public MessageClient(int port)
        {
            this._port = port;
            this._tcpClient = new TcpClient("localhost", port);
            this._bFormatter = new BinaryFormatter();
            this._running = false;
        } // end public MessageClient(int port)

        public void StartListening()
        {
            lock (this)
            {
                if (!_running)
                {
                    this._running = true;
                    this._listenThread = new Thread
                        (new ThreadStart(ListenForMessage));
                    this._listenThread.Start();
                }
                else
                {
                    this._running = true;
                    this._tcpClient = new TcpClient("localhost", this._port);
                    this._listenThread = new Thread
                        (new ThreadStart(ListenForMessage));
                    this._listenThread.Start();
                } // end if (!_running)
            } // end lock (this)
        } // end public void StartListening()

        private void ListenForMessage()
        {
            try
            {
                while (this._running)
                {
                    // Block until an instance Message is received 
                    string message =
                        (string)this._bFormatter.Deserialize
                            (this._tcpClient.GetStream());
                    // Notify all registered event handlers about the message.
                    if (MessageReceived != null && message != null)
                    {
                        MessageReceived(this, message);
                    } // end if MessageReceived != null
                } // end while
            }
            catch (Exception)
            {
                this._running = false;
            } // end try-catch
        } // end private void ListenForMessage();

        public void StopListening()
        {
            lock (this)
            {
                if (this._running)
                {
                    this._tcpClient.Close();
                    _running = false;
                }
            } // end lock(this)
        } // end public void StopListening()

    } // end class MessageClient
}
