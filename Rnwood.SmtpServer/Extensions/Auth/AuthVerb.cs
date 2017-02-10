using Rnwood.SmtpServer.Verbs;
using System.Linq;
using System.Threading.Tasks;

namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class AuthVerb : IVerb
    {
        public AuthVerb(AuthExtensionProcessor authExtensionProcessor)
        {
            AuthExtensionProcessor = authExtensionProcessor;
        }

        public AuthExtensionProcessor AuthExtensionProcessor { get; private set; }

        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            var argumentsParser = new ArgumentsParser(command.ArgumentsText);

            if (argumentsParser.Arguments.Length > 0)
            {
                if (connection.Session.Authenticated)
                {
                    throw new SmtpServerException(new SmtpResponse(StandardSmtpResponseCode.BadSequenceOfCommands,
                                                                   "Already authenticated"));
                }

                var mechanismId = argumentsParser.Arguments[0];
                var mechanism = AuthExtensionProcessor.MechanismMap.Get(mechanismId);

                if (mechanism == null)
                {
                    throw new SmtpServerException(
                        new SmtpResponse(StandardSmtpResponseCode.CommandParameterNotImplemented,
                                         "Specified AUTH mechanism not supported"));
                }

                if (!AuthExtensionProcessor.IsMechanismEnabled(mechanism))
                {
                    throw new SmtpServerException(
                        new SmtpResponse(StandardSmtpResponseCode.AuthenticationFailure,
                                         "Specified AUTH mechanism not allowed right now (might require secure connection etc)"));
                }

                var authMechanismProcessor =
                    mechanism.CreateAuthMechanismProcessor(connection);

                string initialData = null;
                if (argumentsParser.Arguments.Length > 1)
                {
                    initialData = string.Join(" ", argumentsParser.Arguments.Skip(1).ToArray());
                }

                var status =
                    await authMechanismProcessor.ProcessResponseAsync(initialData);
                while (status == AuthMechanismProcessorStatus.Continue)
                {
                    var response = await connection.ReadLineAsync();

                    if (response == "*")
                    {
                        await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.SyntaxErrorInCommandArguments, "Authentication aborted"));
                        return;
                    }

                    status = await authMechanismProcessor.ProcessResponseAsync(response);
                }

                if (status == AuthMechanismProcessorStatus.Success)
                {
                    await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.AuthenticationOk,
                                                              "Authenticated OK"));
                    connection.Session.Authenticated = true;
                    connection.Session.AuthenticationCredentials = authMechanismProcessor.Credentials;
                }
                else
                {
                    await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.AuthenticationFailure,
                                                              "Authentication failure"));
                }
            }
            else
            {
                throw new SmtpServerException(new SmtpResponse(StandardSmtpResponseCode.SyntaxErrorInCommandArguments,
                                                               "Must specify AUTH mechanism as a parameter"));
            }
        }
    }
}