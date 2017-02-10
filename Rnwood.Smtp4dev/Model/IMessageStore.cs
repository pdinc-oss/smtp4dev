using System;
using System.Collections.Generic;

namespace Rnwood.Smtp4dev.Model
{
    public interface IMessageStore
    {
        IEnumerable<ISmtp4DevMessage> Messages { get; }

        event EventHandler<Smtp4DevMessageEventArgs> MessageAdded;

        event EventHandler<Smtp4DevMessageEventArgs> MessageDeleted;

        void AddMessage(ISmtp4DevMessage message);

        void DeleteMessage(ISmtp4DevMessage message);

        IEnumerable<ISmtp4DevMessage> SearchMessages(string searchTerm);

        void DeleteAllMessages();
    }
}