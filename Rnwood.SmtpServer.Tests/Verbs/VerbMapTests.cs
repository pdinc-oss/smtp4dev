using Xunit;
using Moq;
using Rnwood.SmtpServer.Verbs;

namespace Rnwood.SmtpServer.Tests.Verbs
{
    
    public class VerbMapTests
    {
        [Fact]
        public void GetVerbProcessor_RegisteredVerb_ReturnsVerb()
        {
            var verbMap = new VerbMap();
            var verbMock = new Mock<IVerb>();

            verbMap.SetVerbProcessor("verb", verbMock.Object);

            Assert.Same(verbMock.Object, verbMap.GetVerbProcessor("verb"));
        }

        [Fact]
        public void GetVerbProcessor_RegisteredVerbWithDifferentCase_ReturnsVerb()
        {
            var verbMap = new VerbMap();
            var verbMock = new Mock<IVerb>();

            verbMap.SetVerbProcessor("vErB", verbMock.Object);

            Assert.Same(verbMock.Object, verbMap.GetVerbProcessor("VERB"));
        }

        [Fact]
        public void GetVerbProcessor_NoRegisteredVerb_ReturnsNull()
        {
            var verbMap = new VerbMap();

            Assert.Null(verbMap.GetVerbProcessor("VERB"));
        }

        [Fact]
        public void SetVerbProcessor_RegisteredVerbAgainWithNull_ClearsRegistration()
        {
            var verbMap = new VerbMap();
            var verbMock = new Mock<IVerb>();
            verbMap.SetVerbProcessor("verb", verbMock.Object);

            verbMap.SetVerbProcessor("verb", null);

            Assert.Null(verbMap.GetVerbProcessor("verb"));
        }

        [Fact]
        public void SetVerbProcessor_RegisteredVerbAgainDifferentCaseWithNull_ClearsRegistration()
        {
            var verbMap = new VerbMap();
            var verbMock = new Mock<IVerb>();
            verbMap.SetVerbProcessor("verb", verbMock.Object);

            verbMap.SetVerbProcessor("vErb", null);

            Assert.Null(verbMap.GetVerbProcessor("verb"));
        }

        [Fact]
        public void SetVerbProcessor_RegisteredVerbAgain_UpdatesRegistration()
        {
            var verbMap = new VerbMap();
            var verbMock1 = new Mock<IVerb>();
            var verbMock2 = new Mock<IVerb>();
            verbMap.SetVerbProcessor("verb", verbMock1.Object);

            verbMap.SetVerbProcessor("veRb", verbMock2.Object);

            Assert.Same(verbMock2.Object, verbMap.GetVerbProcessor("verb"));
        }
    }
}