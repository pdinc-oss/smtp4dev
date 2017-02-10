using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rnwood.Smtp4dev.Model
{
    public class MessageStore : IMessageStore
    {
        private readonly ConcurrentDictionary<Guid, ISmtp4DevMessage> _messages = new ConcurrentDictionary<Guid, ISmtp4DevMessage>();

        public IEnumerable<ISmtp4DevMessage> Messages => _messages.Values.ToArray();

        public event EventHandler<Smtp4DevMessageEventArgs> MessageAdded;

        public event EventHandler<Smtp4DevMessageEventArgs> MessageDeleted;

        public void DeleteMessage(ISmtp4DevMessage message)
        {
            ISmtp4DevMessage deletedMessage;
            if (_messages.TryRemove(message.Id, out deletedMessage))
            {
                MessageDeleted?.Invoke(this, new Smtp4DevMessageEventArgs(deletedMessage));
            }
        }

        public void AddMessage(ISmtp4DevMessage message)
        {
            if (_messages.TryAdd(message.Id, message))
            {
                MessageAdded?.Invoke(this, new Smtp4DevMessageEventArgs(message));
            }
        }

        public IEnumerable<ISmtp4DevMessage> SearchMessages(string searchTerm)
        {
            return Messages
                .Where(m =>
                    (m.Subject != null && m.Subject.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > -1)
                    || m.To.Any(to => to.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > -1)
                    || m.From.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) > -1
                )
                .OrderByDescending(m => m.ReceivedDate);
        }

        public void DeleteAllMessages()
        {
            foreach (var message in Messages)
            {
                DeleteMessage(message);
            }
        }
    }
}