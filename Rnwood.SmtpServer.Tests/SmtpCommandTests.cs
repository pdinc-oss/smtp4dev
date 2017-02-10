using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    
    public class SmtpCommandTests
    {
        [Fact]
        public void Parsing_SingleToken()
        {
            var command = new SmtpCommand("DATA");
            Assert.True(command.IsValid);
            Assert.Equal("DATA", command.Verb);
            Assert.Equal("", command.ArgumentsText);
        }

        [Fact]
        public void Parsing_ArgsSeparatedBySpace()
        {
            var command = new SmtpCommand("DATA ARGS");
            Assert.True(command.IsValid);
            Assert.Equal("DATA", command.Verb);
            Assert.Equal("ARGS", command.ArgumentsText);
        }

        [Fact]
        public void Parsing_ArgsSeparatedByColon()
        {
            var command = new SmtpCommand("DATA:ARGS");
            Assert.True(command.IsValid);
            Assert.Equal("DATA", command.Verb);
            Assert.Equal("ARGS", command.ArgumentsText);
        }
    }
}