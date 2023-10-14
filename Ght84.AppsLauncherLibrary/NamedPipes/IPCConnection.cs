
using System;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherLibrary.NamedPipes
{
    public interface IPCConnection : IDisposable
    {
        Task Send(string message);
        event EventHandler Disconnected; 
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
