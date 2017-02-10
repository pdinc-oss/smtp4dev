using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    
    public class SessionEventArgsTests
    {
        [Fact]
        public void Session()
        {
            var mocks = new Mocks();

            var s = new SessionEventArgs(mocks.Session.Object);

            Assert.Equal(s.Session, mocks.Session.Object);
        }
    }
}