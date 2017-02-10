using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    
    public class ParameterParserTests
    {
        [Fact]
        public void NoParameters()
        {
            var parameterParser = new ParameterParser(new string[0]);

            Assert.Equal(0, parameterParser.Parameters.Length);
        }

        [Fact]
        public void SingleParameter()
        {
            var parameterParser = new ParameterParser("KEYA=VALUEA");

            Assert.Equal(1, parameterParser.Parameters.Length);
            Assert.Equal(new Parameter("KEYA", "VALUEA"), parameterParser.Parameters[0]);
        }

        [Fact]
        public void MultipleParameters()
        {
            var parameterParser = new ParameterParser("KEYA=VALUEA", "KEYB=VALUEB");

            Assert.Equal(2, parameterParser.Parameters.Length);
            Assert.Equal(new Parameter("KEYA", "VALUEA"), parameterParser.Parameters[0]);
            Assert.Equal(new Parameter("KEYB", "VALUEB"), parameterParser.Parameters[1]);
        }
    }
}