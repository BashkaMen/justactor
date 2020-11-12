using System;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public class RetryBee : AbstractBee<Unit>
    {
        private readonly  TimeSpan? _delay;
        private int _state = 0;

        public RetryBee() {}
        
        public RetryBee(TimeSpan delay)
        {
            _delay = delay;
        }
        
        protected override Task HandleMessage(Unit msg)
        {
            _state++;

            if (_state < 3)
            {
                throw new Exception("go to retry");
            }
            
            return Task.CompletedTask;
        }

        protected override Task<HandleResult> HandleError(BeeMessage<Unit> msg, Exception ex)
        {
            return _delay.HasValue ? HandleResult.RetryTask(_delay.Value) : HandleResult.RetryTask();
        }

        public int GetState()
        {
            return _state;
        }
    }
}