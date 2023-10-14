using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace Ght84.AppsLauncherLibrary.TcpCommunication
{
    // Enumeration that define the type of message
    public enum MessageType { Error, Warning, Info }

    [Serializable]
    public class Message
    {
        private MessageType _messageType;
        private string _messageTitle;
        private string _messageContents;

        public Message(string title, string contents, MessageType type)
        {
            this.Title = title;
            this.Contents = contents;
            this.Type = type;
        }

        public MessageType Type
        {
            get
            {
                return this._messageType;
            }
            set
            {
                this._messageType = value;
            }
        }

        public string Title
        {
            get
            {
                return this._messageTitle;
            }
            set
            {
                this._messageTitle = value;
            }
        }

        public string Contents
        {
            get
            {
                return this._messageContents;
            }
            set
            {
                this._messageContents = value;
            }
        }
    } // end public class Message
}
