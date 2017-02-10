using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Rnwood.SmtpServer.Verbs;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class DataVerbTests
    {
        [Fact]
        public async Task Data_DoubleDots_Unescaped()
        {
            //Check escaping of end of message character ".." is decoded to "."
            //but the .. after B should be left alone
            await TestGoodDataAsync(new string[] { "A", "..", "B..", "." }, "A\r\n.\r\nB..", true);
        }

        [Fact]
        public async Task Data_EmptyMessage_Accepted()
        {
            await TestGoodDataAsync(new string[] { "." }, "", true);
        }

        [Fact]
        public async Task Data_8BitData_TruncatedTo7Bit()
        {
            await TestGoodDataAsync(new string[] { ((char)(0x41 + 128)).ToString(), "." }, "\u0041", false);
        }

        [Fact]
        public async Task Data_8BitData_PassedThrough()
        {
            var data = ((char)(0x41 + 128)).ToString();
            await TestGoodDataAsync(new string[] { data, "." }, data, true);
        }

        private async Task TestGoodDataAsync(string[] messageData, string expectedData, bool eightBitClean)
        {
            var mocks = new Mocks();

            if (eightBitClean)
            {
                mocks.Connection.SetupGet(c => c.ReaderEncoding).Returns(Encoding.UTF8);
            }

            var messageBuilder = new MemoryMessage.Builder();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(messageBuilder);
            mocks.ServerBehaviour.Setup(b => b.GetMaximumMessageSize(It.IsAny<IConnection>())).Returns((long?)null);

            var messageLine = 0;
            mocks.Connection.Setup(c => c.ReadLineAsync()).Returns(() => Task.FromResult(messageData[messageLine++]));

            var verb = new DataVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("DATA"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.StartMailInputEndWithDot);
            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);

            using (var dataReader = new StreamReader(messageBuilder.GetData(), eightBitClean ? Encoding.UTF8 : new AsciiSevenBitTruncatingEncoding()))
            {
                Assert.Equal(expectedData, dataReader.ReadToEnd());
            }
        }

        [Fact]
        public async Task Data_AboveSizeLimit_Rejected()
        {
            var mocks = new Mocks();

            var messageBuilder = new MemoryMessage.Builder();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(messageBuilder);
            mocks.ServerBehaviour.Setup(b => b.GetMaximumMessageSize(It.IsAny<IConnection>())).Returns(10);

            var messageData = new string[] { new string('x', 11), "." };
            var messageLine = 0;
            mocks.Connection.Setup(c => c.ReadLineAsync()).Returns(() => Task.FromResult(messageData[messageLine++]));

            var verb = new DataVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("DATA"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.StartMailInputEndWithDot);
            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.ExceededStorageAllocation);
        }

        [Fact]
        public async Task Data_ExactlySizeLimit_Accepted()
        {
            var mocks = new Mocks();

            var messageBuilder = new MemoryMessage.Builder();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(messageBuilder);
            mocks.ServerBehaviour.Setup(b => b.GetMaximumMessageSize(It.IsAny<IConnection>())).Returns(10);

            var messageData = new string[] { new string('x', 10), "." };
            var messageLine = 0;
            mocks.Connection.Setup(c => c.ReadLineAsync()).Returns(() => Task.FromResult(messageData[messageLine++]));

            var verb = new DataVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("DATA"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.StartMailInputEndWithDot);
            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);
        }

        [Fact]
        public async Task Data_WithinSizeLimit_Accepted()
        {
            var mocks = new Mocks();

            var messageBuilder = new MemoryMessage.Builder();
            mocks.Connection.SetupGet(c => c.CurrentMessage).Returns(messageBuilder);
            mocks.ServerBehaviour.Setup(b => b.GetMaximumMessageSize(It.IsAny<IConnection>())).Returns(10);

            var messageData = new string[] { new string('x', 9), "." };
            var messageLine = 0;
            mocks.Connection.Setup(c => c.ReadLineAsync()).Returns(() => Task.FromResult(messageData[messageLine++]));

            var verb = new DataVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("DATA"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.StartMailInputEndWithDot);
            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);
        }

        [Fact]
        public async Task Data_NoCurrentMessage_ReturnsError()
        {
            var mocks = new Mocks();

            var verb = new DataVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("DATA"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.BadSequenceOfCommands);
        }
    }
}