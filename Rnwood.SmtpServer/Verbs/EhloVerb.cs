#region

using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Rnwood.SmtpServer.Verbs
{
    public class EhloVerb : IVerb
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

            var text = new StringBuilder();
            text.AppendLine("Nice to meet you.");

            foreach (var extnName in connection.ExtensionProcessors.SelectMany(extn => extn.EhloKeywords))
            {
                text.AppendLine(extnName);
            }

            await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.Ok, text.ToString().TrimEnd()));
        }
    }
}