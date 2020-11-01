using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JustActors.Tests.Actors;
using Xunit;
using Xunit.Abstractions;

namespace JustActors.Tests
{
    public class BeeSwarmTests
    {
        private readonly ITestOutputHelper _output;
        private readonly SlowlyBeSwarm _swarm;
        private readonly SlowlyBee _slowlyBee;

        public BeeSwarmTests(ITestOutputHelper output)
        {
            _output = output;

            _swarm = new SlowlyBeSwarm(10);
            _slowlyBee = new SlowlyBee();
        }

        [Fact]
        public void Use_Slowly()
        {
            var seconds = 3;
            var bag = new ConcurrentBag<Task<int>>();
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            while (!cts.IsCancellationRequested)
            {
                bag.Add(_slowlyBee.GetRandomNumber());                
            }

            var completed = bag.Where(s => s.IsCompleted).ToArray();
            
            _output.WriteLine($"RPS: {completed.Length / seconds}");
        }
        
        
        
        [Fact]
        public void Use_SlowlySwarm()
        {
            var seconds = 3;
            var bag = new ConcurrentBag<Task<int>>();
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            while (!cts.IsCancellationRequested)
            {
                bag.Add(_swarm.GetRandomNumber());
            }
            
            var completed = bag.Where(s => s.IsCompleted).ToArray();
            
            _output.WriteLine($"RPS: {completed.Length / seconds}");
        }
    }
}