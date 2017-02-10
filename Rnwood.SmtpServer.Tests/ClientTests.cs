using MailKit.Net.Smtp;
using MimeKit;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public class ClientTests
    {
        [Fact]
        public async Task SmtpClient_NonSSL()
        {
            using (var server = new DefaultServer(false, Ports.AssignAutomatically))
            {
                var messages = new ConcurrentBag<IMessage>();

                server.MessageReceived += (o, ea) =>
                {
                    messages.Add(ea.Message);
                };
                server.Start();

                await SendMessageAsync(server, "to@to.com").WithTimeout("sending message");

                Assert.Equal(1, messages.Count);
                Assert.Equal("from@from.com", messages.First().From);
            }
        }

        [Fact]
        public async Task SmtpClient_NonSSL_StressTest()
        {
            using (var server = new DefaultServer(false, Ports.AssignAutomatically))
            {
                var messages = new ConcurrentBag<IMessage>();

                server.MessageReceived += (o, ea) =>
                {
                    messages.Add(ea.Message);
                };
                server.Start();

                var sendingTasks = new List<Task>();

                const int numberOfThreads = 10;
                const int numberOfMessagesPerThread = 50;

                for (var threadId = 0; threadId < numberOfThreads; threadId++)
                {
                    var localThreadId = threadId;

                    sendingTasks.Add(Task.Run(async () =>
                    {
                        using (var client = new SmtpClient())
                        {
                            await client.ConnectAsync("localhost", server.PortNumber);

                            for (var i = 0; i < numberOfMessagesPerThread; i++)
                            {
                                var message = NewMessage(i + "@" + localThreadId);

                                await client.SendAsync(message);
                                ;
                            }

                            await client.DisconnectAsync(true);
                        }
                    }));
                }

                await Task.WhenAll(sendingTasks).WithTimeout(120, "sending messages");
                Assert.Equal(numberOfMessagesPerThread * numberOfThreads, messages.Count);

                for (var threadId = 0; threadId < numberOfThreads; threadId++)
                {
                    for (var i = 0; i < numberOfMessagesPerThread; i++)
                    {
                        Assert.True(messages.Any(m => m.To.Any(t => t == i + "@" + threadId)));
                    }
                }
            }
        }

        private static async Task SendMessageAsync(DefaultServer server, string toAddress)
        {
            var message = NewMessage(toAddress);

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("localhost", server.PortNumber);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        private static MimeMessage NewMessage(string toAddress)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", "from@from.com"));
            message.To.Add(new MailboxAddress("", toAddress));
            message.Subject = "subject";
            message.Body = new TextPart("plain")
            {
                Text = "body"
            };
            return message;
        }
    }
}