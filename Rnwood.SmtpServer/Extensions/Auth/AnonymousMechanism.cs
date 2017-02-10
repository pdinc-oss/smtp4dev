namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class AnonymousMechanism : IAuthMechanism
    {
        #region IAuthMechanism Members

        public string Identifier => "ANONYMOUS";

        public IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection)
        {
            return new AnonymousMechanismProcessor(connection);
        }

        public bool IsPlainText => false;

        #endregion IAuthMechanism Members
    }
}