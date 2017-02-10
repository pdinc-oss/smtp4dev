namespace Rnwood.SmtpServer.Extensions
{
    public interface IExtensionProcessor
    {
        string[] EhloKeywords
        {
            get;
        }
    }
}