using MimeKit;
using Rnwood.SmtpServer;
using System;

namespace Rnwood.Smtp4dev.Model
{
    internal class Smtp4DevMessage : MemoryMessage, ISmtp4DevMessage
    {
        private Smtp4DevMessage(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        public string Subject { get; private set; }

        public new class Builder : MemoryMessage.Builder
        {
            public Builder() : base(new Smtp4DevMessage(Guid.NewGuid()))
            {
            }

            public override IMessage ToMessage()
            {
                var message = (Smtp4DevMessage)base.ToMessage();

                try
                {
                    var mimeMessage = MimeMessage.Load(message.GetData());
                    message.Subject = mimeMessage.Subject;
                }
                catch (FormatException)
                {
                }

                return message;
            }
        }
    }
}