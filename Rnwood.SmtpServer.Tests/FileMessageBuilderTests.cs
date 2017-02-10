using System.IO;

namespace Rnwood.SmtpServer.Tests
{
    
    public class FileMessageBuilderTests : MessageBuilderTests
    {
        protected override IMessageBuilder GetInstance()
        {
            var tempFile = new FileInfo(Path.GetTempFileName());

            var mocks = new Mocks();
            return new FileMessage.Builder(tempFile, false);
        }
    }
}