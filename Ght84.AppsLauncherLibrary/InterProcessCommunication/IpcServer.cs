using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherLibrary.InterProcessCommunication
{
    public class IpcServer
    {


        private NamedPipeServerStream serverPipe;
        private string pipeName;

        public IpcServer(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public event EventHandler<string> MessageReceivedEventArgs;

        public void StartReceiving()
        {
            Task.Run(() => {
                while (true)
                {
                    this.serverPipe = new NamedPipeServerStream(pipeName);

                    this.serverPipe.WaitForConnection();
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(this.serverPipe))
                    {
                        string content = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(content))
                            MessageReceivedEventArgs?.Invoke(this, content);
                    }
                    if (serverPipe.IsConnected)
                    {
                        serverPipe.Close();
                    }
                }
            });
        }

        //public void SendMessage(string message)
        //{
        //    using (NamedPipeClientStream clientPipe = new NamedPipeClientStream(pipeName))
        //    {
        //        clientPipe.Connect(5000);
        //        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(clientPipe))
        //            writer.Write(message);
        //    }
        //}
    }
}
