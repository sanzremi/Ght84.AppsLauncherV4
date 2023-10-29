using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ght84.AppsLauncherManager.Models
{
    [XmlRoot("AppsLauncherConfiguration")]
    public class AppsLauncherConfiguration
    {
        public string LisezMoi;

        [XmlArray("Executables")]
        [XmlArrayItem("Executable")]
        public List<Executable> Executables { get; set; }

        [XmlArray("Commands")]
        [XmlArrayItem("Command")]
        public List<Command> Commands { get; set; }

        public AppsLauncherConfiguration()
        {
            Executables = new List<Executable>();

            Commands = new List<Command>();
        }

    }




}
