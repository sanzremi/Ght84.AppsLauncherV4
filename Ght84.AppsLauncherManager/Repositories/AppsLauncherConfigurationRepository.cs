using Ght84.AppsLauncherManager.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Ght84.AppsLauncher.Server.Repositories
{
    internal class AppsLauncherConfigurationRepository
    {

        public AppsLauncherConfiguration AppsLauncherConfiguration { get; private set; }

        private string _configXmlFile;
        protected NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        public AppsLauncherConfigurationRepository()
        {
        }

        public void LoadConfigXmlFile(string configXmlFile)
        {
            _configXmlFile = configXmlFile;

            if (!System.IO.File.Exists(configXmlFile)) // Si le fichier config XML n'existe pas 
            {
                throw new Exception($"Le fichier de configuration {configXmlFile} n'est pas accessible ou est absent");
            }

            try 
            { 

                //Lecture du fichier XML de configuration
                string xml = File.ReadAllText(configXmlFile);
                xml = xml.Replace("&", "&amp;");


                XmlSerializer serializer = new XmlSerializer(typeof(AppsLauncherConfiguration));
                StringReader rdr = new StringReader(xml);

                // Désérialisation du fichier XML dans un objet ConfigMenusCommandes
                AppsLauncherConfiguration = (AppsLauncherConfiguration)serializer.Deserialize(rdr);

                // On récupère l'objet Executable pour chaque commande
                foreach(Command c in AppsLauncherConfiguration.Commands) 
                {
                    c.Executable = AppsLauncherConfiguration.Executables.SingleOrDefault(e => e.Code == c.ExecutableCode);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Echec de chargement du fichier de configuration {configXmlFile} : {ex.Message}");
            }


        }


        public Command GetCommandByCode(string code, out ReturnStatus returnStatus)
        {
            Command command = null;

            try
            {
                command = AppsLauncherConfiguration.Commands.Find(x => x.Code == code);
                returnStatus = new ReturnStatus(ReturnStatusEnum.OK);

            }
            catch (Exception e)
            {
                _logger.Error(e);
                returnStatus = new ReturnStatus(ReturnStatusEnum.ERROR, e.Message);
            }

            return command;
        }


    }
}





       

