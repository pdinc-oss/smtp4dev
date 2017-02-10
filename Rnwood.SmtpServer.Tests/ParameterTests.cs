using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public class ParameterTests
    {
        [Fact]
        public void Name()
        {
            var p = new Parameter("name", "value");

            Assert.Equal("name", p.Name);
        }

        [Fact]
        public void Value()
        {
            var p = new Parameter("name", "value");

            Assert.Equal("value", p.Value);
        }

        [Fact]
        public void Equality_Equal()
        {
            Assert.True(new Parameter("KEYA", "VALUEA").Equals(new Parameter("KEYa", "VALUEA")));
        }

        [Fact]
        public void Equality_NotEqual()
        {
            Assert.False(new Parameter("KEYb", "VALUEb").Equals(new Parameter("KEYa", "VALUEA")));
        }
    }
}