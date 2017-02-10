#region

using System.Threading.Tasks;

#endregion

namespace Rnwood.SmtpServer.Verbs
{
    public class RcptVerb : IVerb
    {
        public RcptVerb()
        {
            SubVerbMap = new VerbMap();
            SubVerbMap.SetVerbProcessor("TO", new RcptToVerb());
        }

        public VerbMap SubVerbMap { get; private set; }

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