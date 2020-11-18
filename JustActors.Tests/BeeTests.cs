using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JustActors.Tests.Actors;
using Xunit;

namespace JustActors.Tests
{
    public class BeeTests
    {
        [Fact]
        public void Use_Logger()
        {
            var logger = new LoggerBee();
            logger.Post(new LogMessage("log message")); // manual
            logger.LogMessage("log with helper"); // helper

            
            logger.Post(new FlushMessage()); // manual
            logger.Flush(); // helper
        }


        [Fact]
        public async Task Use_Incrementor()
        {
            var incrementor = new IncrementBee();
            
            var count = 1_000_000;
            for (var i = 0; i < count; i++)
            {
                incrementor.Post(Unit.Value);
            }
            
            await incrementor.WaitEmptyMailBox();
            Assert.Equal(count, incrementor.GetState());
        }
        
        
        [Fact]
        public async Task Use_Summator()
        {
            var summator = new SummatorBee();
            
            var sum = await summator.Sum(5, 5);

            Assert.Equal(10, sum);
        }

        [Fact(Skip = "only manual usage")]
        public async Task Million_Actors()
        {
            var actors = Enumerable.Range(0, 1_000_000).Select(s => new SummatorBee()).ToArray();

            await Task.Delay(10_000);
        }

        [Fact]
        public async Task Use_RetryBee()
        {
            var retryBee = new RetryBee();

            retryBee.Post(Unit.Value);

            await retryBee.WaitEmptyMailBox();
            
            Assert.Equal(3, retryBee.GetState());
        }

    }
}