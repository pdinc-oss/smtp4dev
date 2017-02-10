using Microsoft.Extensions.DependencyInjection;

namespace Rnwood.Smtp4dev.Model
{
    public static class ServiceExtensions
    {
        public static void UseSmtp4Dev(this IServiceCollection services)
        {
            var settingsStore = new SettingsStore();
            services.AddSingleton<ISettingsStore>(settingsStore);

            var messageStore = new MessageStore();
            services.AddSingleton<IMessageStore>(messageStore);

            services.AddTransient<ISmtp4DevEngine, Smtp4DevEngine>();
        }
    }
}