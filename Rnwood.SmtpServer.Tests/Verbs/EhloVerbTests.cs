using Moq;
using Rnwood.SmtpServer.Extensions;
using System.Threading.Tasks;
using Rnwood.SmtpServer.Verbs;
using Xunit;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    public class EhloVerbTests
    {
        [Fact]
        public async Task Process_RespondsWith250()
        {
            var mocks = new Mocks();
            var mockExtensionProcessor1 = new Mock<IExtensionProcessor>();
            mockExtensionProcessor1.SetupGet(ep => ep.EhloKeywords).Returns(new[] { "EXTN1" });
            var mockExtensionProcessor2 = new Mock<IExtensionProcessor>();
            mockExtensionProcessor2.SetupGet(ep => ep.EhloKeywords).Returns(new[] { "EXTN2A", "EXTN2B" });

            mocks.Connection.SetupGet(c => c.ExtensionProcessors).Returns(new[]
                                                                              {
                                                                                  mockExtensionProcessor1.Object,
                                                                                  mockExtensionProcessor2.Object
                                                                              });

            var ehloVerb = new EhloVerb();
            await ehloVerb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("EHLO foobar"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);
        }

        [Fact]
        public async Task Process_NoArguments_Accepted()
        {
            var mocks = new Mocks();
            var ehloVerb = new EhloVerb();
            await ehloVerb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("EHLO"));
            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);

            mocks.Session.VerifySet(s => s.ClientName = "");
        }

        [Fact]
        public async Task Process_RecordsClientName()
        {
            var mocks = new Mocks();
            var ehloVerb = new EhloVerb();
            await ehloVerb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("EHLO foobar"));

            mocks.Session.VerifySet(s => s.ClientName = "foobar");
        }

        [Fact]
        public async Task Process_RespondsWithExtensionKeywords()
        {
            var mocks = new Mocks();
            var mockExtensionProcessor1 = new Mock<IExtensionProcessor>();
            mockExtensionProcessor1.SetupGet(ep => ep.EhloKeywords).Returns(new[] { "EXTN1" });
            var mockExtensionProcessor2 = new Mock<IExtensionProcessor>();
            mockExtensionProcessor2.SetupGet(ep => ep.EhloKeywords).Returns(new[] { "EXTN2A", "EXTN2B" });

            mocks.Connection.SetupGet(c => c.ExtensionProcessors).Returns(new[]
                                                                              {
                                                                                  mockExtensionProcessor1.Object,
                                                                                  mockExtensionProcessor2.Object
                                                                              });

            var ehloVerb = new EhloVerb();
            await ehloVerb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("EHLO foobar"));

            mocks.Connection.Verify(c => c.WriteResponseAsync(It.Is<SmtpResponse>(r =>

                r.Message.Contains("EXTN1") &&
                r.Message.Contains("EXTN2A") &&
                    r.Message.Contains("EXTN2B")
                )));
        }

        [Fact]
        public async Task Process_SaidHeloAlready_Allowed()
        {
            var mocks = new Mocks();

            var verb = new EhloVerb();
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("EHLO foo.blah"));
            await verb.ProcessAsync(mocks.Connection.Object, new SmtpCommand("EHLO foo.blah"));

            mocks.VerifyWriteResponseAsync(StandardSmtpResponseCode.Ok);
        }
    }
}