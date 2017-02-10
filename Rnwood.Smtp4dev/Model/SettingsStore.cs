using System;

namespace Rnwood.Smtp4dev.Model
{
    public class SettingsStore : ISettingsStore
    {
        private Settings _settings = new Settings();

        public Settings Load()
        {
            return _settings;
        }

        public void Save(Settings settings)
        {
            _settings = settings;

            Saved?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Saved;
    }
}