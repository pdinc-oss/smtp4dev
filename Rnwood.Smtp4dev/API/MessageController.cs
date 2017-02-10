using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rnwood.Smtp4dev.API.DTO;
using Rnwood.Smtp4dev.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Rnwood.Smtp4dev.API
{
    [Route("api/message")]
    public class MessagesController : Controller
    {
        private readonly IMessageStore _messageStore;

        public MessagesController(IMessageStore messageStore)
        {
            _messageStore = messageStore;
        }

        [HttpGet("{searchTerm?}")]
        public IEnumerable<Message> Get(string searchTerm)
        {
            return !string.IsNullOrEmpty(searchTerm)
                ? _messageStore.SearchMessages(searchTerm).Select(m => new Message(m))
                : _messageStore.Messages.Select(m => new Message(m));
        }

        [HttpGet("{id}")]
        public Message Get(Guid id)
        {
            return _messageStore.Messages
                .Where(m => m.Id == id)
                .Select(m => new Message(m))
                .FirstOrDefault();
        }

        [HttpDelete("{id?}")]
        public IActionResult Delete(Guid? id)
        {
            if (id.HasValue)
            {
                var message = _messageStore.Messages.FirstOrDefault(m => m.Id == id);

                if (message != null)
                {
                    _messageStore.DeleteMessage(message);
                }
            }
            else
            {
                _messageStore.DeleteAllMessages();
            }

            return new NoContentResult();
        }

        [HttpGet("events")]
        public async Task Events()
        {
            HttpContext.Response.ContentType = "text/event-stream";

            var messagesChangedEvent = new AutoResetEvent(false);

            _messageStore.MessageAdded += (s, ea) =>
            {
                messagesChangedEvent.Set();
            };

            _messageStore.MessageDeleted += (s, ea) =>
            {
                messagesChangedEvent.Set();
            };

            while (true)
            {
                await messagesChangedEvent.WaitOneAsync();
                await HttpContext.Response.WriteAsync("event: messageschanged\ndata: messages changed!\n\n");
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}