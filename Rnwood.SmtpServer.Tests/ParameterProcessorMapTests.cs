using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    public class ParameterProcessorMapTests
    {
        [Fact]
        public void GetProcessor_NotRegistered_Null()
        {
            var map = new ParameterProcessorMap();
            Assert.Null(map.GetProcessor("BLAH"));
        }

        [Fact]
        public void GetProcessor_Registered_Returned()
        {
            var processor = new Mock<IParameterProcessor>();

            var map = new ParameterProcessorMap();
            map.SetProcessor("BLAH", processor.Object);

            Assert.Same(processor.Object, map.GetProcessor("BLAH"));
        }

        [Fact]
        public void GetProcessor_RegisteredDifferentCase_Returned()
        {
            var processor = new Mock<IParameterProcessor>();

            var map = new ParameterProcessorMap();
            map.SetProcessor("blah", processor.Object);

            Assert.Same(processor.Object, map.GetProcessor("BLAH"));
        }

        [Fact]
        public async Task Process_UnknownParameter_Throws()
        {
            var e = await Assert.ThrowsAsync<SmtpServerException>(async () =>
           {
               var mocks = new Mocks();

               var map = new ParameterProcessorMap();
               await map.ProcessAsync(mocks.Connection.Object, new string[] { "KEYA=VALUEA" }, true);
           });

            Assert.Equal("Parameter KEYA is not recognised", e.Message);
        }

        [Fact]
        public async Task Process_NoParameters_Accepted()
        {
            var mocks = new Mocks();

            var map = new ParameterProcessorMap();
            await map.ProcessAsync(mocks.Connection.Object, new string[] { }, true);
        }

        [Fact]
        public async Task Process_KnownParameters_Processed()
        {
            var mocks = new Mocks();
            var keyAProcessor = new Mock<IParameterProcessor>();
            var keyBProcessor = new Mock<IParameterProcessor>();

            var map = new ParameterProcessorMap();
            map.SetProcessor("keya", keyAProcessor.Object);
            map.SetProcessor("keyb", keyBProcessor.Object);

            await map.ProcessAsync(mocks.Connection.Object, new string[] { "KEYA=VALUEA", "KEYB=VALUEB" }, true);

            keyAProcessor.Verify(p => p.SetParameter(mocks.Connection.Object, "KEYA", "VALUEA"));
            keyBProcessor.Verify(p => p.SetParameter(mocks.Connection.Object, "KEYB", "VALUEB"));
        }
    }
}