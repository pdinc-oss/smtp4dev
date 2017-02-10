using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    
    public class MessageEventArgsTests
    {
        [Fact]
        public void Message()
        {
            IMessage message = new MemoryMessage();
            var messageEventArgs = new MessageEventArgs(message);

            Assert.Same(message, messageEventArgs.Message);
        }
    }
}