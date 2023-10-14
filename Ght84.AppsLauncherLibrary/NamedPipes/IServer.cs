using System;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherLibrary.NamedPipes
{
    public interface IServer : IPCConnection
    {
        Task Start();
        event EventHandler ServerStarted;
        event EventHandler ClientConnected;
    }
}
