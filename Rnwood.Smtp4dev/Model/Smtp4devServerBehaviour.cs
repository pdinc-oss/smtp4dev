using Rnwood.SmtpServer;
using Rnwood.SmtpServer.Extensions;
using Rnwood.SmtpServer.Extensions.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Rnwood.Smtp4dev.Model
{
    internal class Smtp4DevServerBehaviour : IServerBehaviour
    {
        internal Smtp4DevServerBehaviour(Settings settings, Action<ISmtp4DevMessage> messageRecievedHandler)
        {
            _settings = settings;
            _messageReceivedHandler = messageRecievedHandler;
        }

        private readonly Action<ISmtp4DevMessage> _messageReceivedHandler;
        private readonly Settings _settings;

        public string DomainName => "smtp4dev";

        public IPAddress IpAddress => IPAddress.Any;

        public int MaximumNumberOfSequentialBadCommands => 10;

        public int PortNumber => _settings.Port;

        public Encoding GetDefaultEncoding(IConnection connection)
        {
            return new AsciiSevenBitTruncatingEncoding();
        }

        public IEnumerable<IExtension> GetExtensions(IConnection connection)
        {
            return Enumerable.Empty<IExtension>();
        }

        public long? GetMaximumMessageSize(IConnection connection)
        {
            return null;
        }

        public TimeSpan GetReceiveTimeout(IConnection connection)
        {
            return TimeSpan.FromMinutes(5);
        }

        public TimeSpan GetSendTimeout(IConnection connection)
        {
            return TimeSpan.FromMinutes(5);
        }

        public X509Certificate GetSslCertificate(IConnection connection)
        {
            return null;
        }

        public bool IsAuthMechanismEnabled(IConnection connection, IAuthMechanism authMechanism)
        {
            return true;
        }

        public bool IsSessionLoggingEnabled(IConnection connection)
        {
            return true;
        }

        public bool IsSslEnabled(IConnection connection)
        {
            return false;
        }

        public void OnCommandReceived(IConnection connection, SmtpCommand command)
        {
        }

        public IMessageBuilder OnCreateNewMessage(IConnection connection)
        {
            return new Smtp4DevMessage.Builder();
        }

        public IEditableSession OnCreateNewSession(IConnection connection, IPAddress clientAddress, DateTime startDate)
        {
            return new MemorySession(clientAddress, startDate);
        }

        public void OnMessageCompleted(IConnection connection)
        {
        }

        public void OnMessageReceived(IConnection connection, IMessage message)
        {
            _messageReceivedHandler((ISmtp4DevMessage)message);
        }

        public void OnMessageRecipientAdding(IConnection connection, IMessageBuilder message, string recipient)
        {
        }

        public void OnMessageStart(IConnection connection, string from)
        {
        }

        public void OnSessionCompleted(IConnection connection, ISession session)
        {
        }

        public void OnSessionStarted(IConnection connection, ISession session)
        {
        }

        public Task<AuthenticationResult> ValidateAuthenticationCredentialsAsync(IConnection connection, IAuthenticationCredentials authenticationRequest)
        {
            return Task.FromResult(AuthenticationResult.Success);
        }
    }
}