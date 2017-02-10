using Rnwood.SmtpServer;
using System;

namespace Rnwood.Smtp4dev.Model
{
    public class Smtp4DevEngine : ISmtp4DevEngine
    {
        private Server _server;
        private readonly ISettingsStore _settingsStore;
        private readonly IMessageStore _messageStore;

        public event EventHandler StateChanged;

        public Smtp4DevEngine(ISettingsStore settingsStore, IMessageStore messageStore)
        {
            _settingsStore = settingsStore;
            _settingsStore.Saved += OnSettingsChanged;
            _messageStore = messageStore;

            TryStart();
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            ApplySettings();
        }

        public Exception ServerError { get; set; }

        public bool IsRunning => _server != null && _server.IsRunning;

        private void ApplySettings()
        {
            if (_server != null)
            {
                if (_server.IsRunning)
                {
                    Stop();
                }

                if (_server != null)
                {
                    _server.IsRunningChanged -= OnServerStateChanged;
                }

                _server = null;
            }

            TryStart();
        }

        public void TryStart()
        {
            ServerError = null;
            var settings = _settingsStore.Load();

            if (settings.IsEnabled)
            {
                try
                {
                    _server = new Server(new Smtp4DevServerBehaviour(settings, OnMessageReceived));
                    _server.IsRunningChanged += OnServerStateChanged;
                    _server.Start();
                }
                catch (Exception e)
                {
                    ServerError = e;
                }
            }
        }

        private void Stop()
        {
            _server?.Stop(true);
        }

        private void OnServerStateChanged(object sender, EventArgs eventArgs)
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMessageReceived(ISmtp4DevMessage message)
        {
            _messageStore.AddMessage(message);
        }
    }
}