using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ght84.AppsLauncherLibrary.UdpCommunication;
using Ght84.AppsLauncherLibrary.Helpers;

namespace Ght84.AppsLauncherLibrary.TcpCommunication
{
    public class MessageServer
    {
        private int _port;
        private TcpListener _tcpListener;
        private Dictionary<string, TcpClient> _clientsDictionary;
        private bool _running;
        private bool _disposed;

        private BinaryFormatter _bFormatter;

        private Thread _connectionThread;

        // Create a message server that listens on the indicated port
        public MessageServer(int port)
        {
            this._port = port;
            this._tcpListener = new TcpListener(IPAddress.Loopback, port);
            this._clientsDictionary = new Dictionary<string, TcpClient>();
            this._running = false;
            this._bFormatter = new BinaryFormatter();
        } // end public MessageServer(int port) 

        public void Start()
        {
            if (!_running)
            {
                this._tcpListener.Start();
                this._running = true;
                this._connectionThread = new Thread
                    (new ThreadStart(ListenForClientConnections));
                this._connectionThread.Start();
            } // end if (!_running)

        } // end public void Start()

        public void Stop()
        {
            if (this._running)
            {
                this._tcpListener.Stop();
                this._running = false;
            }
        } // end public void Stop()

        public bool Running()
        {
            return this._running;
        } // end public bool Running()

        // Thread body for listening for client connections
        private void ListenForClientConnections()
        {
            while (this._running)
            {
                TcpClient connectedTcpClient = this._tcpListener.AcceptTcpClient();
                // Remember the current client connection
                //string activeUsername = Machine.GetInstance().GetUsername();
                string activeUsername = "SYSTEM";
                WindowsSession windowsSession = WindowsSessionHelper.GetActiveWindowsSession();
                if (windowsSession != null)
                {
                    activeUsername = windowsSession.UserName;
                }

                lock (this)
                {
                    // If there is another connection from a same client
                    if (this._clientsDictionary.ContainsKey(activeUsername))
                    {
                        // close the connection.
                        this._clientsDictionary[activeUsername].Close();
                        // Remember the new connection
                        this._clientsDictionary[activeUsername] = connectedTcpClient;
                    }
                    // Else 
                    else
                    {
                        // Remember the new connection
                        this._clientsDictionary.Add(activeUsername,
                            connectedTcpClient);
                    } // end if

                } // end lock(this._clientsDictionary)
            } // end while(this._running)
        } // end private void ListenForClientConnections()

        // Send a message to the currently logged in user
        public void SendMessageToActiveClient(string message)
        {
            lock (this)
            {
                // Get the current active user
                //string activeUsername = Machine.GetInstance().GetUsername();
                string activeUsername = "SYSTEM";
                WindowsSession windowsSession = WindowsSessionHelper.GetActiveWindowsSession();
                if (windowsSession != null)
                {
                    activeUsername = windowsSession.UserName;
                }


                // If client had connected to the message server
                if (this._clientsDictionary.ContainsKey(activeUsername))
                {
                    try
                    {
                        // send message to client
                        this.sendMessage
                            (this._clientsDictionary[activeUsername], message);
                    }
                    catch (Exception)
                    {
                        // close the client connection
                        this._clientsDictionary[activeUsername].Close();
                        // Remove the client connection from memory
                        this._clientsDictionary.Remove(activeUsername);
                    } // end try-catch
                } // end if
            } // end lock
        } // end public void sendMessageToActiveClient()

        private void sendMessage(TcpClient client, string message)
        {
            // Send message to the client.
            _bFormatter.Serialize(client.GetStream(), message);
        } // end private void sendMessage(TcpClient client, Message message)

    } // end public class MessageServer


}
