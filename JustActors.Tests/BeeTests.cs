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
            logger.LogMessage("log message"); 
            logger.LogMessage("log with helper"); 
            logger.Flush(); 
        }


        [Fact]
        public async Task Use_Incrementor()
        {
            var incrementor = new IncrementBee();
            
            var count = 1_000_000;
            for (var i = 0; i < count; i++)
            {
                incrementor.Increment();
            }
            
            await incrementor.WaitEndWork();
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

            retryBee.Run();

            await retryBee.WaitEndWork();
            
            Assert.Equal(3, retryBee.GetState());
        }


        [Fact]
        public async Task WaitEmptyMailBox()
        {
            var counter = new SlowlyCounter();

            for (int i = 0; i < 100; i++)
            {
                counter.Increment();
            }

            await counter.WaitEndWork();
            Assert.Equal(100, counter.GetState());

        }
    }
}