namespace Rnwood.SmtpServer.Tests
{
    
    public class MemoryMessageBuilderTests : MessageBuilderTests
    {
        protected override IMessageBuilder GetInstance()
        {
            var mocks = new Mocks();
            return new MemoryMessage.Builder();
        }
    }
}