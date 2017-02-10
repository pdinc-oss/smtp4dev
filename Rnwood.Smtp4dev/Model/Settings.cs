namespace Rnwood.Smtp4dev.Model
{
    public class Settings
    {
        public Settings()
        {
            Port = 5151;
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }
        public int Port { get; set; }
    }
}