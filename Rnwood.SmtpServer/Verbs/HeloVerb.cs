#region

using System.Threading.Tasks;

#endregion

namespace Rnwood.SmtpServer.Verbs
{
    public class HeloVerb : IVerb
    {
        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (!string.IsNullOrEmpty(connection.Session.ClientName))
            {
                await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.BadSequenceOfCommands,
                                                                   "You already said HELO"));
                return;
            }

            connection.Session.ClientName = command.ArgumentsText ?? "";
            await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.Ok, "Nice to meet you"));
        }
    }
}