using System.Threading.Tasks;
using Rnwood.SmtpServer.Verbs;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class QuitVerbTests
    {
        [Fact]
        public async Task Quit_RespondsWithClosingChannel()
        {
            var mocks = new Mocks();

            var quitVerb = new QuitVerb();
            await quitVerb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("QUIT"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.ClosingTransmissionChannel);
        }
    }
}