using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using JustActors.Tests.Actors;
using Xunit;
using Xunit.Abstractions;

namespace JustActors.Tests
{
    public class BeeTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public BeeTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }
        
        
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

            var waiter = Enumerable.Range(0, 3).Select(s => counter.WaitEndWork()).ToArray();
            
            await Task.WhenAll(waiter);

            waiter.All(s => s.IsCompletedSuccessfully).Should().Be(true);
            Assert.Equal(100, counter.GetState());
        }

        [Fact]
        public async Task Timeout()
        {
            var bee = new TimeoutBee();

            var res = await bee.Run();
            res.Should().Be(Unit.Value);

            var res2 = await bee.Run(TimeSpan.FromMilliseconds(300));
            res2.Should().Be(res);
        }

        [Fact]
        public async Task Calc_Message_LifeTime()
        {
            var bee = new SummatorBee();
            
            var sw = Stopwatch.StartNew();
            var res = await bee.Sum(2, 2);
            
            var elapsed = sw.Elapsed;
            _outputHelper.WriteLine($"Elapsed: {elapsed}");
        }
    }
}