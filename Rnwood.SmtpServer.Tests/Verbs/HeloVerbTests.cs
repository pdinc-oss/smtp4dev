using System.Threading.Tasks;
using Rnwood.SmtpServer.Verbs;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class HeloVerbTests
    {
        [Fact]
        public async Task SayHelo()
        {
            var mocks = new Mocks();

            var verb = new HeloVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("HELO foo.blah"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);
            mocks.Session.VerifySet(s => s.ClientName = "foo.blah");
        }

        [Fact]
        public async Task SayHeloTwice_ReturnsError()
        {
            var mocks = new Mocks();
            mocks.Session.SetupGet(s => s.ClientName).Returns("already.said.helo");

            var verb = new HeloVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("HELO foo.blah"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.BadSequenceOfCommands);
        }

        [Fact]
        public async Task SayHelo_NoName()
        {
            var mocks = new Mocks();

            var verb = new HeloVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("HELO"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);
            mocks.Session.VerifySet(s => s.ClientName = "");
        }
    }
}