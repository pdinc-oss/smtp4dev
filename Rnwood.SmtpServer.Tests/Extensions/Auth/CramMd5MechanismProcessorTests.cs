using Moq;
using Rnwood.SmtpServer.Extensions.Auth;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Extensions.Auth
{
    public class CramMd5MechanismProcessorTests : AuthMechanismTest
    {
        [Fact]
        public async Task ProcessRepsonse_GetChallenge()
        {
            var mocks = new Mocks();

            var cramMd5MechanismProcessor = Setup(mocks);
            var result = await cramMd5MechanismProcessor.ProcessResponseAsync(null);

            var expectedResponse = string.Format("{0}.{1}@{2}", Fakerandom, Fakedatetime, Fakedomain);

            Assert.Equal(AuthMechanismProcessorStatus.Continue, result);
            mocks.Connection.Verify(
                    c => c.WriteResponseAsync(
                        It.Is<SmtpResponse>(r =>
                            r.Code == (int)StandardSmtpResponseCode.AuthenticationContinue &&
                            VerifyBase64Response(r.Message, expectedResponse)
                        )
                    )
                );
        }

        [Fact]
        public async Task ProcessRepsonse_ChallengeReponse_BadFormat()
        {
            await Assert.ThrowsAsync<SmtpServerException>(async () =>
            {
                var mocks = new Mocks();

                var challenge = string.Format("{0}.{1}@{2}", Fakerandom, Fakedatetime, Fakedomain);

                var cramMd5MechanismProcessor = Setup(mocks, challenge);
                var result = await cramMd5MechanismProcessor.ProcessResponseAsync("BLAH");
            });
        }

        [Fact]
        public async Task ProcessResponse_Response_BadBase64()
        {
            await Assert.ThrowsAsync<BadBase64Exception>(async () =>
            {
                var mocks = new Mocks();

                var cramMd5MechanismProcessor = Setup(mocks);
                await cramMd5MechanismProcessor.ProcessResponseAsync(null);
                await cramMd5MechanismProcessor.ProcessResponseAsync("rob blah");
            });
        }

        private const int Fakedatetime = 10000;
        private const int Fakerandom = 1234;
        private const string Fakedomain = "mockdomain";

        private CramMd5MechanismProcessor Setup(Mocks mocks, string challenge = null)
        {
            var randomMock = new Mock<IRandomIntegerGenerator>();
            randomMock.Setup(r => r.GenerateRandomInteger(It.IsAny<int>(), It.IsAny<int>())).Returns(Fakerandom);

            var dateMock = new Mock<ICurrentDateTimeProvider>();
            dateMock.Setup(d => d.GetCurrentDateTime()).Returns(new DateTime(Fakedatetime));

            mocks.ServerBehaviour.SetupGet(b => b.DomainName).Returns(Fakedomain);

            return new CramMd5MechanismProcessor(mocks.Connection.Object, randomMock.Object, dateMock.Object, challenge);
        }
    }
}