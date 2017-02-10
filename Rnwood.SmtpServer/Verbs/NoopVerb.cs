using System.Threading.Tasks;

namespace Rnwood.SmtpServer.Verbs
{
    public class NoopVerb : IVerb
    {
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.Ok, "Successfully did nothing"));
        }
    }
}