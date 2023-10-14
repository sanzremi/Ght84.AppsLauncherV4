﻿using System;

namespace Ght84.AppsLauncherLibrary.NamedPipes
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }

        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }
}
