
using System;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherLibrary.NamedPipes
{
    public interface IClient : IPCConnection
    {
        Task Connect();
        event EventHandler ConnectedToServer;
        event EventHandler ClientStarted;
    }
}
