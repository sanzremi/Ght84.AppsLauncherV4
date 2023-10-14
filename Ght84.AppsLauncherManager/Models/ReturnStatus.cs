using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ght84.AppsLauncherManager.Models
{
    public enum ReturnStatusEnum
    {
        OK,
        WARNING,
        ERROR
    }

    public class ReturnStatus
    {
        public string Code { get; private set; }
        public ReturnStatusEnum Status { get; private set; }
        public string Message { get; private set; }
        public int? LastIdInserted { get; private set; }

        public string StatusFormatString
        {
            get
            {
                return Status.ToString(); // Convertit en chaine de caractères OK, WARNING ou ERROR
            }
        }

        public ReturnStatus(ReturnStatusEnum status, string message = "", string code = "", int? lastIdInserted = null)
        {
            Status = status;
            Code = code;
            Message = message;
            LastIdInserted = lastIdInserted;
        }


        public void LogToNLog(NLog.Logger logger)
        {
            if (Status == ReturnStatusEnum.OK)
            {
                logger.Debug(Message);
            }
            else if (Status == ReturnStatusEnum.WARNING)
            {
                logger.Warn(Message);
            }
            else if (Status == ReturnStatusEnum.ERROR)
            {
                logger.Error(Message);
            }
        }

        public string DisplayMessageForUser()
        {

            if (Status == ReturnStatusEnum.ERROR)
            {
                return "Oups! Une erreur technique est survenue. Si le problème persiste veuillez contacter le support informatique. Détail de l'erreur :" + Message;
            }
            else if (Status == ReturnStatusEnum.WARNING)
            {
                return "Avertissement :" + Message;
            }

            else // ReturnStatusEnum.OK
            {
                return "";
            }
        }

    }
}
