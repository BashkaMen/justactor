using System;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public class IncrementBee : AbstractBee<Unit>
    {
        private int _state = 0;

        public int GetState() => _state;
        public void Increment() => Post(Unit.Value);
        public Task WaitEndWork() => WaitEmptyWindow();

        protected override Task HandleMessage(Unit msg)
        {
            _state++;
            
            return Task.CompletedTask;
        }

        protected override Task<HandleResult> HandleError(BeeMessage<Unit> msg, Exception ex)
        {
            return HandleResult.OkTask();
        }
    }

    public class SlowlyCounter : AbstractBee<Unit>
    {
        private int _state = 0;

        public int GetState() => _state;

        public void Increment()
        {
            Post(Unit.Value);
        }

        public Task WaitEndWork() => WaitEmptyWindow();
        
        protected override async Task HandleMessage(Unit msg)
        {
            await Task.Delay(50);
            _state++;
        }

        protected override Task<HandleResult> HandleError(BeeMessage<Unit> msg, Exception ex)
        {
            return HandleResult.OkTask();
        }
    }
}