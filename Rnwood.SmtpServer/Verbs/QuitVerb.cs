#region

using System.Threading.Tasks;

#endregion

namespace Rnwood.SmtpServer.Verbs
{
    public class QuitVerb : IVerb
    {
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.ClosingTransmissionChannel,
                                                               "Goodbye"));
            await connection.CloseConnectionAsync();
            connection.Session.CompletedNormally = true;
        }
    }
}