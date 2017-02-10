using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace Rnwood.Smtp4dev.API.DTO
{
    public class MessagePart
    {
        public MessagePart(int id, MimeEntity messagePart)
        {
            Id = id;
            Headers = messagePart.Headers.ToDictionary(h => h.Field, h => h.Value);
        }

        public MessagePart(int id, string parserError)
        {
            Id = id;
            ParserError = parserError;

            Headers = new Dictionary<string, string>();
        }

        public int Id { get; private set; }
        public IDictionary<string, string> Headers { get; private set; }

        public string ParserError { get; private set; }
    }
}