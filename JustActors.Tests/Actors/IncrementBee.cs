using System;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public class IncrementBee : AbstractBee<Unit>
    {
        private int _state = 0;

        public int GetState() => _state;

        protected override Task HandleMessage(Unit msg)
        {
            _state++;
            
            return Task.CompletedTask;
        }

        protected override Task HandleError(BeeMessage<Unit> msg, Exception ex)
        {
            Console.WriteLine(ex);
            
            return Task.CompletedTask;
        }
    }
}