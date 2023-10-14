using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace Ght84.AppsLauncherDispatcher
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AppsLauncherDispatcher()
            };

            if (Environment.UserInteractive)
            {
                RunInteractive(ServicesToRun);
            }
            else
            {
                ServiceBase.Run(ServicesToRun);
            }
        }
      

        static void RunInteractive(ServiceBase[] servicesToRun)
        {
            Console.WriteLine("Le service Ght84.AppsLauncherDispatcher est démarré en mode intéractif.");
            Console.WriteLine();

            MethodInfo onStartMethod = typeof(ServiceBase).GetMethod("OnStart",
                BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write("Démarrage {0}...", service.ServiceName);
                onStartMethod.Invoke(service, new object[] { new string[] { } });
                Console.Write("Démarré");
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(
                "Presser une touche pour arrêter les services et terminer les process...");
            Console.ReadKey();
            Console.WriteLine();

            MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop",
                BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write("Arrêt en cours {0}...", service.ServiceName);
                onStopMethod.Invoke(service, null);
                Console.WriteLine("Arrété");
            }

            Console.WriteLine("Tous les services sont arrétés.");
            // Keep the console alive for a second to allow the user to see the message.
            Thread.Sleep(1000);
        }

    }
}
