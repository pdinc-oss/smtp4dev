using Rnwood.SmtpServer;
using System;

namespace Rnwood.Smtp4dev.Model
{
    public interface ISmtp4DevMessage : IMessage
    {
        Guid Id { get; }

        string Subject { get; }
    }
}