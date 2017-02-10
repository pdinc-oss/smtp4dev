using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public class TcpClientConnectionChannelTests
    {
        [Fact]
        public async Task ReadLineAsync_ThrowsOnConnectionClose()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);

            try
            {
                listener.Start();
                var acceptTask = listener.AcceptTcpClientAsync();

                var client = new TcpClient();
                await client.ConnectAsync(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndpoint).Port);

                using (var serverTcpClient = await acceptTask)
                {
                    var channel = new TcpClientConnectionChannel(serverTcpClient);
                    client.Dispose();

                    await Assert.ThrowsAsync<ConnectionUnexpectedlyClosedException>(async () =>
                    {
                        await channel.ReadLineAsync();
                    });
                }
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}