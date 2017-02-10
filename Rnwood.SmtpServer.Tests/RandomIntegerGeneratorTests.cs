using Xunit;

namespace Rnwood.SmtpServer.Tests
{
    
    public class RandomIntegerGeneratorTests
    {
        [Fact]
        public void GenerateRandomInteger()
        {
            var randomNumberGenerator = new RandomIntegerGenerator();
            var randomNumber = randomNumberGenerator.GenerateRandomInteger(-100, 100);
            Assert.True(randomNumber >= -100);
            Assert.True(randomNumber <= 100);
        }
    }
}