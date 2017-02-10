#region

#endregion

namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class PlainMechanism : IAuthMechanism
    {
        #region IAuthMechanism Members

        public string Identifier => "PLAIN";

        public IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection)
        {
            return new PlainMechanismProcessor(connection);
        }

        public bool IsPlainText => true;

        #endregion
    }
}