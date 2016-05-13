#region

using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Rnwood.SmtpServer;
using MimeKit;
using Rnwood.Smtp4dev.MessageInspector;

#endregion

namespace Rnwood.Smtp4dev
{
    public class MessageViewModel
    {
        public MessageViewModel(Message message)
        {
            Message = message;
        }

        public Message Message { get; private set; }

        public string From
        {
            get { return Message.From; }
        }

        public string To
        {
            get { return string.Join(", ", Message.To); }
        }

        public DateTime ReceivedDate
        {
            get { return Message.ReceivedDate; }
        }

        public string Subject
        {
            get { return Parts.Subject; }
        }

        private MimeMessage _contents;
        public MimeMessage Parts
        {
            get
            {
                if (_contents == null)
                {
                    _contents = MimeMessage.Load(Message.GetData());
                }

                return _contents;
            }
        }

        public bool HasBeenViewed { get; private set; }

        public void SaveToFile(FileInfo file)
        {
            HasBeenViewed = true;

            byte[] data = new byte[64*1024];
            int bytesRead;

            using (Stream dataStream = Message.GetData(false))
            {
                using (FileStream fileStream = file.OpenWrite())
                {
                    while ((bytesRead = dataStream.Read(data, 0, data.Length)) > 0)
                    {
                        fileStream.Write(data, 0, bytesRead);
                    }
                }
            }
        }

        public void SaveToFileText(FileInfo file)
        {
            var body = string.IsNullOrEmpty(Parts.TextBody) ? Parts.HtmlBody : Parts.TextBody;
            var attachmentsList = Parts.Attachments.OfType<MimePart>().Select(x => x.FileName).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("From: " + Parts.From);
            sb.AppendLine("To: " + Parts.To);
            sb.AppendLine("Date: " + Parts.Date);
            sb.AppendLine("Subject: " + Parts.Subject);
            sb.AppendLine("Body: " + body);

            if (attachmentsList.Any())
                sb.AppendLine("AttachmentNames: " + string.Join(",", attachmentsList));

            File.AppendAllText(file.FullName, sb.ToString());
        }

        public void SaveAttachments(DirectoryInfo directory, string filePrefix)
        {
            foreach (var attachment in Parts.Attachments.OfType<MimePart>())
            {
                var file = new FileInfo(Path.Combine(directory.FullName, filePrefix + "__" + attachment.FileName));

                using (var fileStream = file.OpenWrite())
                {
                    attachment.ContentObject.DecodeTo(fileStream);
                }
            }
        }

        public void SaveToFileWithAttachments(FileInfo msgFile, DirectoryInfo attachmentsDirectory, string attachmentsFilePrefix)
        {
            SaveToFileText(msgFile);
            SaveAttachments(attachmentsDirectory, attachmentsFilePrefix);
        }

        public void MarkAsViewed()
        {
            HasBeenViewed = true;
        }
    }
}