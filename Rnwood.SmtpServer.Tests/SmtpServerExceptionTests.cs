using Xunit;
using System;

namespace Rnwood.SmtpServer.Tests
{
    
    public class SmtpServerExceptionTests
    {
        [Fact]
        public void InnerException()
        {
            var innerException = new Exception();

            var e = new SmtpServerException(new SmtpResponse(StandardSmtpResponseCode.ExceededStorageAllocation, "Blah"), innerException);

            Assert.Same(innerException, e.InnerException);
        }

        [Fact]
        public void SmtpResponse()
        {
            var smtpResponse = new SmtpResponse(StandardSmtpResponseCode.ExceededStorageAllocation, "Blah");
            var e = new SmtpServerException(smtpResponse);

            Assert.Same(smtpResponse, e.SmtpResponse);
        }
    }
}