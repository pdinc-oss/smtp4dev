using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rnwood.Smtp4dev.API.DTO;
using Rnwood.Smtp4dev.Model;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rnwood.Smtp4dev.API
{
    [Route("api/server")]
    public class ServerController : Controller
    {
        public ServerController(ISmtp4DevEngine engine, ISettingsStore settingsStore)
        {
            _engine = engine;
            _settingsStore = settingsStore;
        }

        private readonly ISmtp4DevEngine _engine;
        private readonly ISettingsStore _settingsStore;

        // GET: api/values
        [HttpGet("{id}")]
        public Server Get(int id)
        {
            var settings = _settingsStore.Load();

            return new Server()
            {
                IsRunning = _engine.IsRunning,
                Error = _engine?.ServerError?.Message,
                Port = settings.Port,
                IsEnabled = settings.IsEnabled
            };
        }

        [HttpPut("{id}")]
        public Server Update([FromBody] ServerUpdate server)
        {
            var settings = _settingsStore.Load();

            settings.Port = server.Port;
            settings.IsEnabled = server.IsEnabled;
            _settingsStore.Save(settings);

            return Get(server.Id);
        }

        [HttpGet("events")]
        public async Task Events()
        {
            HttpContext.Response.ContentType = "text/event-stream";

            var stateChangedEvent = new AutoResetEvent(false);

            _engine.StateChanged += (s, ea) =>
            {
                stateChangedEvent.Set();
            };

            while (true)
            {
                await stateChangedEvent.WaitOneAsync();
                await HttpContext.Response.WriteAsync("event: statechanged\ndata: stateChange!\n\n");
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}