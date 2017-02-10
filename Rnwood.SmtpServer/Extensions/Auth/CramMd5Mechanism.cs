#region

#endregion

namespace Rnwood.SmtpServer.Extensions.Auth
{
    public class CramMd5Mechanism : IAuthMechanism
    {
        #region IAuthMechanism Members

        public string Identifier => "CRAM-MD5";

        public IAuthMechanismProcessor CreateAuthMechanismProcessor(IConnection connection)
        {
            return new CramMd5MechanismProcessor(connection, new RandomIntegerGenerator(), new CurrentDateTimeProvider());
        }

        public bool IsPlainText => false;

        #endregion
    }
}