using Moq;
using Rnwood.SmtpServer.Verbs;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public async Task Process_GreetingWritten()
        {
            var mocks = new Mocks();
            mocks.ConnectionChannel.Setup(c => c.WriteLineAsync(It.IsAny<string>())).Callback(() => mocks.Connection.Object.CloseConnectionAsync().Wait());

            var connection = new Connection(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object);
            await connection.ProcessAsync();

            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.IsRegex("220 .*", RegexOptions.IgnoreCase)));
        }

        [Fact]
        public async Task Process_SmtpServerExceptionThrow_ResponseWritten()
        {
            var mocks = new Mocks();
            var mockVerb = new Mock<IVerb>();
            mocks.VerbMap.Setup(v => v.GetVerbProcessor(It.IsAny<string>())).Returns(mockVerb.Object);
            mockVerb.Setup(v => v.ProcessAsync(It.IsAny<IConnection>(), It.IsAny<SmtpCommand>())).Returns(Task.FromException(new SmtpServerException(new SmtpResponse(500, "error"))));

            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("GOODCOMMAND").Callback(() => mocks.Connection.Object.CloseConnectionAsync().Wait());

            var connection = new Connection(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object);
            await connection.ProcessAsync();

            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.IsRegex("500 error", RegexOptions.IgnoreCase)));
        }

        [Fact]
        public async Task Process_EmptyCommand_NoResponse()
        {
            var mocks = new Mocks();

            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("").Callback(() => mocks.Connection.Object.CloseConnectionAsync().Wait());

            var connection = new Connection(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object);
            await connection.ProcessAsync();

            //Should only print service ready message
            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.Is<string>(s => !s.StartsWith("220 "))), Times.Never());
        }

        [Fact]
        public async Task Process_GoodCommand_Processed()
        {
            var mocks = new Mocks();
            var mockVerb = new Mock<IVerb>();
            mocks.VerbMap.Setup(v => v.GetVerbProcessor(It.IsAny<string>())).Returns(mockVerb.Object).Callback(() => mocks.Connection.Object.CloseConnectionAsync().Wait());

            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("GOODCOMMAND");

            var connection = new Connection(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object);
            await connection.ProcessAsync();

            mockVerb.Verify(v => v.ProcessAsync(It.IsAny<IConnection>(), It.IsAny<SmtpCommand>()));
        }

        [Fact]
        public async Task Process_BadCommand_500Response()
        {
            var mocks = new Mocks();
            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("BADCOMMAND").Callback(() => mocks.Connection.Object.CloseConnectionAsync().Wait());

            var connection = new Connection(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object);
            await connection.ProcessAsync();

            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.IsRegex("500 .*", RegexOptions.IgnoreCase)));
        }

        [Fact]
        public async Task Process_TooManyBadCommands_Disconnected()
        {
            var mocks = new Mocks();
            mocks.ServerBehaviour.SetupGet(b => b.MaximumNumberOfSequentialBadCommands).Returns(2);

            mocks.ConnectionChannel.Setup(c => c.ReadLineAsync()).ReturnsAsync("BADCOMMAND");

            var connection = new Connection(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object);
            await connection.ProcessAsync();

            mocks.ConnectionChannel.Verify(c => c.ReadLineAsync(), Times.Exactly(2));
            mocks.ConnectionChannel.Verify(cc => cc.WriteLineAsync(It.IsRegex("221 .*", RegexOptions.IgnoreCase)));
        }

        [Fact]
        public void AbortMessage()
        {
            var mocks = new Mocks();

            var connection = new Connection(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object);
            connection.NewMessage();

            connection.AbortMessage();
            Assert.Null(connection.CurrentMessage);
        }

        [Fact]
        public void CommitMessage()
        {
            var mocks = new Mocks();

            var connection = new Connection(mocks.Server.Object, mocks.ConnectionChannel.Object, mocks.VerbMap.Object);
            var messageBuilder = connection.NewMessage();
            var message = messageBuilder.ToMessage();

            connection.CommitMessage();
            mocks.Session.Verify(s => s.AddMessage(message));
            mocks.ServerBehaviour.Verify(b => b.OnMessageReceived(connection, message));
            Assert.Null(connection.CurrentMessage);
        }
    }
}