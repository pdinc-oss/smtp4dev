#region

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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

        ///
        ///  
        class Setting
        {
            public Setting()
            { }

            public Setting(string keyValuePair, string keyValuePairDelimeter = "=")
            {
                var items = keyValuePair.Split(new string[] { keyValuePairDelimeter }, StringSplitOptions.None);
                Key = items[0];
                Value = items[1];
            }

            public static Setting Create(string key, string value)
            {
                var retVal = new Setting { Key = key, Value = value };
                return retVal;
            }

            public string Value { get; set; }

            public string Key { get; set; }
        }

        class SimpleSettings
        {
            // e.g. called from command line like so...
            // "port=25 mailsFolder=c:\temp\mails clearMailsOnStartup=false"
            public SimpleSettings()
            {
                Settings = new List<Setting>();
            }

            // e.g. stored in app config line like so...
            // <add key="singleSettingsLine" value="port=25 mailsFolder=c:\temp\mails clearMailsOnStartup=false" />
            public void AddCommandlineArgs(string[] cmdLineArgs, string keyPrefixFilter = "", string keyValuePairDelimiter = "=")
            {
                foreach (var item in cmdLineArgs.Where(x=>x.StartsWith(keyPrefixFilter)))
                {
                    Settings.Add(new Setting(item, keyValuePairDelimiter));
                }
            }

            // e.g. stored in app config line like so...
            // <add key="settings" value="port=25 mailsFolder=c:\temp\mails clearMailsOnStartup=false" />
            public void AddSingleLineSettings(string allSettingsInsideSingleString, string keyPrefixFilter = "", string keyValuePairDelimiter = "=", string settingDelimiter = " ")
            {
                var items = allSettingsInsideSingleString.Split(new string[] { settingDelimiter },
                    StringSplitOptions.RemoveEmptyEntries);
                AddCommandlineArgs(items, keyPrefixFilter, keyValuePairDelimiter);
            }

            // e.g. stored in app config line like so...
            // <add key="myProg.port" value="port=25" />
            // <add key="myProg.mailsFolder" value="c:\temp\mail" />
            // <add key="myProg.clearMailsOnStartup" value="false" />
            public void AddAppConfigSettings(string keyPrefixFilter = "")
            {
                foreach (var key in ConfigurationManager.AppSettings.AllKeys)
                {
                    var addTheSetting = true;
                    if (!string.IsNullOrEmpty(keyPrefixFilter))
                    {
                        if (!key.StartsWith(keyPrefixFilter))
                            addTheSetting = false;
                    }

                    if (addTheSetting)
                        Settings.Add(Setting.Create(key, ConfigurationManager.AppSettings[key]));
                }
            }

            public void AddAppConfigSetting(string appConfigKey)
            {
                Settings.Add(Setting.Create(appConfigKey, ConfigurationManager.AppSettings[appConfigKey]));
            }

            public void AddTextConfigSettings(string textConfigFile, string keyPrefixFilter = "", string keyValuePairDelimiter = "=")
            {
                var lines = File.ReadAllLines(textConfigFile);
               AddCommandlineArgs(lines, keyPrefixFilter, keyValuePairDelimiter);
            }

            public List<Setting> Settings { get; set; }

            public T Get<T>(string key, Func<string, T> convertFunc, T defaultValue = null) where T : class
            {
                foreach (var setting in Settings)
                {
                    if (setting.Key == key)
                    {
                        var retVal = convertFunc(setting.Value);
                        return retVal;
                    }
                }
                return defaultValue;
            }

            public string GetString(string key, string defaultValue = null)
            {
                foreach (var setting in Settings)
                {
                    if (setting.Key == key)
                    {
                        return setting.Value;
                    }
                }
                return defaultValue;
            }

            public int GetInt(string key, int defaultValue = -1)
            {
                foreach (var setting in Settings)
                {
                    if (setting.Key == key)
                    {
                        return Int32.Parse(setting.Value);
                    }
                }
                return defaultValue;
            }

            public bool GetBool(string key, bool defaultValue = false)
            {
                foreach (var setting in Settings)
                {
                    if (setting.Key == key)
                    {
                        return bool.Parse(setting.Value);
                    }
                }
                return defaultValue;
            }

            public T GetEnum<T>(string key, T defaultValue) where T : struct, IConvertible
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");

                foreach (var setting in Settings)
                {
                    if (setting.Key == key)
                    {
                        return (T)Enum.Parse(typeof(T), setting.Value);
                    }
                }
                return defaultValue;
            }
        }

        class AppConfig
        {
            public AppConfig()
            { }

            public AppConfig(string[] args)
            {
                var settings = new SimpleSettings();
                settings.AddCommandlineArgs(args);
                settings.AddTextConfigSettings("simpleSmtpServer.config.txt");
                settings.AddAppConfigSettings("simpleSmtpServer.");
                MailsFolder = settings.GetString("simpleSmtpServer.mailsFolder");
                ClearMailsOnStartup = settings.GetBool("simpleSmtpServer.clearMailsOnStartup");
                Port = settings.GetEnum("port", Ports.SMTP);
            }

            public bool ClearMailsOnStartup { get; set; }

            public Ports Port { get; set; }

            public string MailsFolder { get; set; }
        }

        private static AppConfig config;
        private static void Main(string[] args)
        {
            config = new AppConfig(args);
            if (string.IsNullOrEmpty(config.MailsFolder))
                throw new ConfigurationErrorsException("mailsFolder is null or empty");
            if (!Directory.Exists(config.MailsFolder))
                Directory.CreateDirectory(config.MailsFolder);

            if (config.ClearMailsOnStartup)
            {
                Console.WriteLine("Clearing mail drop folder "   + config.MailsFolder);
                ClearFolder(config.MailsFolder);
            }
            List<IMessage> messages = new List<IMessage>();
            //DefaultServer server = new DefaultServer(Ports.AssignAutomatically);
            DefaultServer server = new DefaultServer(config.Port);
            server.SessionCompleted += SessionCompleted;
            server.MessageReceived += MessageReceived;
            server.SessionStarted += SessionStarted;
            server.Start();

            //do something to send mail
            Console.WriteLine("Listening on localhost at port " + server.PortNumber);
            Console.WriteLine("Will write mails to folder " + config.MailsFolder);
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
            var fullFileName = Path.Combine(config.MailsFolder, fileName.Name);

            WriteMailToDisk(fullFileName, parsedMail);
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