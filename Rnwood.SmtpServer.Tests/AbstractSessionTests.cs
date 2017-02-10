using Moq;
using System;
using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public abstract class AbstractSessionTests
    {
        protected abstract IEditableSession GetSession();

        [Fact]
        public void AppendToLog()
        {
            var session = GetSession();
            session.AppendToLog("Blah1");
            session.AppendToLog("Blah2");

            Assert.Equal(new[] { "Blah1", "Blah2", "" },
                                    session.GetLog().ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.None));
        }

        [Fact]
        public void GetMessages_InitiallyEmpty()
        {
            var session = GetSession();
            Assert.Equal(0, session.GetMessages().Length);
        }

        [Fact]
        public void AddMessage()
        {
            var session = GetSession();
            var message = new Mock<IMessage>();

            session.AddMessage(message.Object);

            Assert.Equal(1, session.GetMessages().Length);
            Assert.Same(message.Object, session.GetMessages()[0]);
        }
    }
}