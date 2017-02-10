using System;

namespace Rnwood.SmtpServer
{
    public class RandomIntegerGenerator : IRandomIntegerGenerator
    {
        private static readonly Random Random = new Random();

        public int GenerateRandomInteger(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }
    }
}