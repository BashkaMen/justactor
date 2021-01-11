using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public class SlowlyBee : AbstractBee<ReplyChannel<int>>
    {
        private readonly Random _random = new Random();


        public Task<int> GetRandomNumber() => PostAndReply<int>(rc => rc);
        
        
        protected override async Task HandleMessage(ReplyChannel<int> msg)
        {
            await Task.Delay(500);
            msg.Reply(_random.Next(100, 500));
        }

        protected override Task<HandleResult> HandleError(BeeMessage<ReplyChannel<int>> msg, Exception ex)
        {
            return HandleResult.OkTask();
        }
    }

    public class SlowlyBeSwarm
    {
        private readonly SlowlyBee[] _bees;

        public SlowlyBeSwarm(int count)
        {
            _bees = Enumerable.Range(0, count).Select(s => new SlowlyBee()).ToArray();
        }


        public Task<int> GetRandomNumber() => _bees.Random().GetRandomNumber();
    }
}