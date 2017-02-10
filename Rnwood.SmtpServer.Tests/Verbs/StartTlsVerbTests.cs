using Moq;
using Rnwood.SmtpServer.Extensions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class StartTlsVerbTests
    {
        [Fact]
        public async Task NoCertificateAvailable_ReturnsErrorResponse()
        {
            var mocks = new Mocks();
            mocks.ServerBehaviour.Setup(b => b.GetSslCertificate(It.IsAny<IConnection>())).Returns<X509Certificate>(null);

            var verb = new StartTlsVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("STARTTLS"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.CommandNotImplemented);
        }
    }
}