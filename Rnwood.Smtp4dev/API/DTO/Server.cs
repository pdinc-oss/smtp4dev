namespace Rnwood.Smtp4dev.API.DTO
{
    public class Server
    {
        public int Id { get; set; }

        public bool IsRunning { get; internal set; }

        public bool IsEnabled { get; set; }

        public string Error { get; set; }
        public int Port { get; set; }
    }
}