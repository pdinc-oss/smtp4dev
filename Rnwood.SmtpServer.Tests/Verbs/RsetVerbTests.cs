using Rnwood.SmtpServer.Verbs;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class RsetVerbTests
    {
        [Fact]
        public async Task ProcessAsync()
        {
            var mocks = new Mocks();

            var verb = new RsetVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("RSET"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);
            mocks.Connection.Verify(c => c.AbortMessage());
        }
    }
}