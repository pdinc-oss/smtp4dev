#region

using System.Threading.Tasks;

#endregion

namespace Rnwood.SmtpServer.Verbs
{
    public class MailVerb : IVerb
    {
        public MailVerb()
        {
            SubVerbMap = new VerbMap();
            SubVerbMap.SetVerbProcessor("FROM", new MailFromVerb());
        }

        public VerbMap SubVerbMap { get; private set; }

        public MailFromVerb FromSubVerb => (MailFromVerb)SubVerbMap.GetVerbProcessor("FROM");

        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            var subrequest = new SmtpCommand(command.ArgumentsText);
            var verbProcessor = SubVerbMap.GetVerbProcessor(subrequest.Verb);

            if (verbProcessor != null)
            {
                await verbProcessor.ProcessAsync(connection, subrequest);
            }
            else
            {
                await connection.WriteResponseAsync(
                    new SmtpResponse(StandardSmtpResponseCode.CommandParameterNotImplemented,
                                     "Subcommand {0} not implemented", subrequest.Verb));
            }
        }
    }
}