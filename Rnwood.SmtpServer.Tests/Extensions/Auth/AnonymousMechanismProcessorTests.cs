using Moq;
using Rnwood.SmtpServer.Extensions.Auth;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Extensions.Auth
{
    public class AnomymousMechanismProcessorTests
    {
        [Fact]
        public async Task ProcessResponse_Success()
        {
            await ProcessResponseAsync(AuthenticationResult.Success, AuthMechanismProcessorStatus.Success);
        }

        [Fact]
        public async Task ProcessResponse_Failure()
        {
            await ProcessResponseAsync(AuthenticationResult.Failure, AuthMechanismProcessorStatus.Failed);
        }

        [Fact]
        public async Task ProcessResponse_TemporarilyFailure()
        {
            await ProcessResponseAsync(AuthenticationResult.TemporaryFailure, AuthMechanismProcessorStatus.Failed);
        }

        private async Task ProcessResponseAsync(AuthenticationResult authenticationResult, AuthMechanismProcessorStatus authMechanismProcessorStatus)
        {
            var mocks = new Mocks();
            mocks.ServerBehaviour.Setup(
                b =>
                b.ValidateAuthenticationCredentialsAsync(mocks.Connection.Object, It.IsAny<AnonymousAuthenticationCredentials>()))
                .ReturnsAsync(authenticationResult);

            var anonymousMechanismProcessor = new AnonymousMechanismProcessor(mocks.Connection.Object);
            var result = await anonymousMechanismProcessor.ProcessResponseAsync(null);

            Assert.Equal(authMechanismProcessorStatus, result);

            if (authenticationResult == AuthenticationResult.Success)
            {
                Assert.IsType(typeof(AnonymousAuthenticationCredentials), anonymousMechanismProcessor.Credentials);
            }
        }
    }
}