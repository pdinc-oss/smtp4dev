using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    
    public class ArgumentsParserTests
    {
        [Fact]
        public void Parsing_FirstArgumentAferVerbWithColon_Split()
        {
            var args = new ArgumentsParser("ARG1=VALUE:BLAH");
            Assert.Equal(1, args.Arguments.Length);
            Assert.Equal("ARG1=VALUE:BLAH", args.Arguments[0]);
        }

        [Fact]
        public void Parsing_MailFrom_WithDisplayName()
        {
            var args = new ArgumentsParser("<Robert Wood<rob@rnwood.co.uk>> ARG1 ARG2");
            Assert.Equal("<Robert Wood<rob@rnwood.co.uk>>", args.Arguments[0]);
            Assert.Equal("ARG1", args.Arguments[1]);
            Assert.Equal("ARG2", args.Arguments[2]);
        }

        [Fact]
        public void Parsing_MailFrom_EmailOnly()
        {
            var args = new ArgumentsParser("<rob@rnwood.co.uk> ARG1 ARG2");
            Assert.Equal("<rob@rnwood.co.uk>", args.Arguments[0]);
            Assert.Equal("ARG1", args.Arguments[1]);
            Assert.Equal("ARG2", args.Arguments[2]);
        }
    }
}