using System;
using MimeKit;
using Rnwood.Smtp4dev.Model;

namespace Rnwood.Smtp4dev.API.DTO
{
    public class Message
    {
        private readonly ISmtp4DevMessage _message;

        public Message(ISmtp4DevMessage message)
        {
            _message = message;

            using (var messageData = message.GetData())
            {
                try
                {
                    var mimeMessage = MimeMessage.Load(messageData);
                    Subject = mimeMessage.Subject;
                    Body = mimeMessage.HtmlBody ?? string.Empty;
                }
                catch (FormatException)
                {
                    Subject = "";
                }
            }
        }

        public DateTime ReceivedDate => _message.ReceivedDate;

        public string From => _message.From;

        public string[] To => _message.To;

        public string Subject { get; private set; }

        public Guid Id => _message.Id;

        public string Body { get; set; }
    }
}