using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherManager.Models
{
    public class Executable
    {

        // Code de l'exécutable à lancer (exemple NOTEPAD, CHROME, EDICTEE, ...)
        public string Code { get; set; }

        // Chemin de l'exécutable (exemple C:\Program Files\Mozilla Firefox\firefox.exe)
        public string TargetPath { get; set; }

        // Autre chemin possible de l'exécutable si pas trouvé dans TargetPath (exemple C:\Program Files\Mozilla Firefox\firefox.exe)
        public string AlternativeTargetPath { get; set; }

        //Délai de Timeout en milisecondes : Délai à attendre maximun afin que le Handle de la fenêtre principale de la commande soit active
        // Au dela du Timeout, le lancement de la commande sera considéré en échec
        //Valeur moyenne à prévoir entre 500 et 2000 ms
        public int? TimeoutMainWindow { get; set; }


    }
}
