using Microsoft.Extensions.Logging;

namespace Rnwood.SmtpServer
{
    public class Logging
    {
        private static readonly ILoggerFactory _factory = new LoggerFactory();

        public static ILoggerFactory Factory => _factory;
    }
}