using System;

namespace Rnwood.Smtp4dev.Model
{
    public class Smtp4DevMessageEventArgs : EventArgs
    {
        public Smtp4DevMessageEventArgs(ISmtp4DevMessage message)
        {
            Message = message;
        }

        public ISmtp4DevMessage Message { get; private set; }
    }
}