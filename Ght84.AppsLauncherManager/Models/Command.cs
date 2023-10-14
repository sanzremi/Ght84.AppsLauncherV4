using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ght84.AppsLauncherManager.Models
{
    public class Command
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Label { get; set; }

        public string Comment { get; set; }

        public string Type { get; set; }

        public string ExecutableCode { get; set; }

        public Executable Executable { get; set; }

        public string Arguments { get; set; }

        public string InformationMessage { get; set; }

        public string WarningMessage { get; set; }

        public string ADGroups { get; set; }

    }
}
