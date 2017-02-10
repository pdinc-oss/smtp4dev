using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    
    public class CommandEventArgsTests
    {
        [Fact]
        public void Command()
        {
            var command = new SmtpCommand("BLAH");
            var args = new CommandEventArgs(command);

            Assert.Same(command, args.Command);
        }
    }
}