#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace Rnwood.SmtpServer.Example
{
    /// <summary>
    /// A simple example use of Rnwood.SmtpServer.
    /// Prints a message to the console when a session is established, completed
    /// or a message is received.
    /// </summary>
    internal class Program
    {
        private static string mailsFolder = string.Empty;
        private static void Main(string[] args)
        {
            mailsFolder = ConfigurationManager.AppSettings["mailsFolder"];
            if (string.IsNullOrEmpty(mailsFolder))
                throw new ConfigurationErrorsException("mailsFolder is null or empty");
            if (!Directory.Exists(mailsFolder))
                Directory.CreateDirectory(mailsFolder);

            var clearMailsFolderOnStartup = bool.Parse(ConfigurationManager.AppSettings["clearMailsOnStartup"]);
            if (clearMailsFolderOnStartup)
                ClearFolder(mailsFolder);
            List<IMessage> messages = new List<IMessage>();
            //DefaultServer server = new DefaultServer(Ports.AssignAutomatically);
            Ports port = (Ports)Enum.Parse(typeof(Ports), ConfigurationManager.AppSettings["port"]);
            DefaultServer server = new DefaultServer(port);
            server.SessionCompleted += SessionCompleted;
            server.MessageReceived += MessageReceived;
            server.SessionStarted += SessionStarted;
            server.Start();

            //do something to send mail
            Console.WriteLine("Listening on localhost at port " + server.PortNumber);
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            server.Stop();
        }

        private static void ClearFolder(string folderPath)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(folderPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private static void SessionCompleted(object sender, SessionEventArgs e)
        {
            Console.WriteLine(string.Format("SESSION END - Address:{0} NoOfMessages:{1} Error:{2}",
                                            e.Session.ClientAddress, e.Session.GetMessages().Length, e.Session.SessionError));
        }

        private static void SessionStarted(object sender, SessionEventArgs e)
        {
            Console.WriteLine(string.Format("SESSION START - Address:{0}", e.Session.ClientAddress));
        }

        static int mailIdx = 0;
        private static void MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine(string.Format("MESSAGE RECEIVED - Envelope From:{0} Envelope To:{1}", e.Message.From,
                                            string.Join(", ", e.Message.To)));

            var parsedMail = new ParsedMail(e);
            var fileName = new MailFileName(parsedMail);
            var fullFileName = Path.Combine(mailsFolder, fileName.Name);

            WriteMailToDisk(fullFileName, parsedMail);

            //    //If you wanted to write the message out to a file, then could do this...
            //    mailIdx++;
            //using (var inStream = e.Message.GetData())
            //{
            //    var mailFileName =
            //    var file = Path.Combine(mailsFolder, "mail" + mailIdx + ".eml");
            //    using (var outStream = File.Create(file))
            //    {
            //        byte[] buffer = new byte[1024];
            //        int bytesRead = 0;
            //        while ((bytesRead = inStream.Read(buffer, 0, buffer.Length)) > 0)
            //        {
            //            outStream.Write(buffer, 0, bytesRead);
            //        }
            //        outStream.Flush();
            //    }
            //}

            //var outStream = new MemoryStream();
            //var r = new StreamReader(outStream);
            //r.re
            //r
            ////File.WriteAllBytes("myfile" + mailIdx + ".eml", e.Message.GetData().);
        }

        private static void WriteMailToDisk(string fullFileName, ParsedMail parsedMail)
        {
            using (var outStream = File.Create(fullFileName))
            {
                using (var writer = new StreamWriter(outStream))
                {
                    foreach (var line in parsedMail.Content.Lines)
                    {
                        writer.WriteLine(line);
                    }
                    writer.Flush();
                }
            }
        }

        private static string GetFileName(MessageEventArgs e)
        {
            var parsedMail = new ParsedMail(e);
            var fileName = new MailFileName(parsedMail);
            return fileName.Name;
        }

        class MailFileName
        {
            public MailFileName()
            { }

            public MailFileName(ParsedMail mail)
            {
                //fromuser_domain_to_touser_domain_subject
                Name = string.Format("{0}-{1} to {2}-{3} [{4}].eml",
                    mail.FromUser.UserName,
                    mail.FromUser.UserDomain,
                    mail.ToUser.UserName,
                    mail.ToUser.UserDomain,
                    mail.Subject.Text);
            }

            public string Name { get; set; }
        }

        class MailUser
        {
            public MailUser(string fromAddress)
            {
                var items = fromAddress.Split(new char[] { '@' });
                UserName = items[0];
                UserDomain = items[1];
            }

            public MailUser()
            { }

            public string UserName { get; set; }
            public string UserDomain { get; set; }
        }

        class MailSubject
        {
            public MailSubject()
            {

            }

            public MailSubject(MailContent mailContent)
            {
                foreach (var line in mailContent.Lines)
                {
                    if (line.StartsWith("Subject: "))
                    {
                        var items = line.Split(new string[] { "Subject: " }, StringSplitOptions.None);
                        if (items.Length != 2)
                            throw new ApplicationException("Was expecting a valid subject line");
                        Text = items[1];
                        break;
                    }
                }
            }

            public string Text { get; set; }
        }

        class MailContent
        {
            public MailContent()
            { }

            public MailContent(Stream data)
            {
                var text = new List<string>();
                using (var sr = new StreamReader(data))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        text.Add(line);
                    }
                }
                Lines = text;
            }

            public List<string> Lines { get; set; }
        }

        class ParsedMail
        {
            public ParsedMail()
            { }

            public ParsedMail(MessageEventArgs args)
            {
                FromUser = new MailUser(args.Message.From);
                ToUser = new MailUser(args.Message.To[0]);
                using (var stream = args.Message.GetData())
                {
                    Content = new MailContent(stream);
                }
                Subject = new MailSubject(Content);
            }

            public MailSubject Subject { get; set; }

            public MailUser ToUser { get; set; }

            public MailUser FromUser { get; set; }

            public MailContent Content { get; set; }
        }
    }
}