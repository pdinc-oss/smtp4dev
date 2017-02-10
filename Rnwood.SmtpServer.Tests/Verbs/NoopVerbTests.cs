using Rnwood.SmtpServer.Verbs;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class NoopVerbTests
    {
        [Fact]
        public async Task Noop()
        {
            var mocks = new Mocks();

            var verb = new NoopVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("NOOP"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);
        }
    }
}