using System;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public class RetryBee : AbstractBee<Unit>
    {
        private int _state = 0;

        public RetryBee() {}

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
            return HandleResult.RetryTask();
        }

        public int GetState()
        {
            return _state;
        }
    }
}