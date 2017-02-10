using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    
    public class SmtpResponseTests
    {
        [Fact]
        public void IsError_Error()
        {
            var r = new SmtpResponse(500, "An error happened");
            Assert.True(r.IsError);
        }

        [Fact]
        public void IsError_NotError()
        {
            var r = new SmtpResponse(200, "No error happened");
            Assert.False(r.IsError);
        }

        [Fact]
        public void IsSuccess_Error()
        {
            var r = new SmtpResponse(500, "An error happened");
            Assert.False(r.IsSuccess);
        }

        [Fact]
        public void IsSuccess_NotError()
        {
            var r = new SmtpResponse(200, "No error happened");
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void Message()
        {
            var r = new SmtpResponse(1, "Blah");
            Assert.Equal("Blah", r.Message);
        }

        [Fact]
        public void Code()
        {
            var r = new SmtpResponse(1, "Blah");
            Assert.Equal(1, r.Code);
        }

        [Fact]
        public void ToString_SingleLineMessage()
        {
            var r = new SmtpResponse(200, "Single line message");
            Assert.Equal("200 Single line message\r\n", r.ToString());
        }

        [Fact]
        public void ToString_MultiLineMessage()
        {
            var r = new SmtpResponse(200, "Multi line message line 1\r\n" +
            "Multi line message line 2\r\n" +
            "Multi line message line 3");
            Assert.Equal("200-Multi line message line 1\r\n" +
            "200-Multi line message line 2\r\n" +
            "200 Multi line message line 3\r\n", r.ToString());
        }

        [Fact]
        public void Equality_Equal()
        {
            Assert.True(new SmtpResponse(StandardSmtpResponseCode.Ok, "OK").Equals(new SmtpResponse(StandardSmtpResponseCode.Ok, "OK")));
        }

        [Fact]
        public void Equality_NotEqual()
        {
            Assert.False(new SmtpResponse(StandardSmtpResponseCode.SyntaxErrorCommandUnrecognised, "Eror").Equals(new SmtpResponse(StandardSmtpResponseCode.Ok, "OK")));
        }
    }
}