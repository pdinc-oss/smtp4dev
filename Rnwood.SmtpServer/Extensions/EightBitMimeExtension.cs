#region

#endregion

using Rnwood.SmtpServer.Verbs;

namespace Rnwood.SmtpServer.Extensions
{
    public class EightBitMimeExtension : IExtension
    {
        public EightBitMimeExtension()
        {
        }

        public IExtensionProcessor CreateExtensionProcessor(IConnection connection)
        {
            return new EightBitMimeExtensionProcessor(connection);
        }

        #region Nested type: EightBitMimeExtensionProcessor

        private class EightBitMimeExtensionProcessor : ExtensionProcessor
        {
            public EightBitMimeExtensionProcessor(IConnection connection)
                : base(connection)
            {
                var verb = new EightBitMimeDataVerb();
                connection.VerbMap.SetVerbProcessor("DATA", verb);

                var mailVerbProcessor = connection.MailVerb;
                var mailFromProcessor = mailVerbProcessor.FromSubVerb;
                mailFromProcessor.ParameterProcessorMap.SetProcessor("BODY", verb);
            }

            public override string[] EhloKeywords => new[] { "8BITMIME" };
        }

        #endregion
    }
}