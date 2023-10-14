using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherLibrary.InterProcessCommunication
{
    public class IpcClient
    {

        private string pipeName;

        public IpcClient(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public void SendMessage(string message)
        {
            using (NamedPipeClientStream clientPipe = new NamedPipeClientStream(pipeName))
            {
                clientPipe.Connect(2000);
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(clientPipe))
                { 
                    writer.Write(message);
                }                
            }
        }
    }
}
