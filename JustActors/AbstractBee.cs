using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JustActors
{
    public interface IBee {}
    
    public abstract class AbstractBee<T> : IBee
    {
        private readonly ActionBlock<BeeMessage<T>> _mailbox;
        
        protected bool IsBusy { get; private set; }
        public AbstractBee()
        {
            
            _mailbox = new ActionBlock<BeeMessage<T>>(async msg =>
            {
                try
                {
                    await Handle(msg);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,
                EnsureOrdered = true,
                MaxMessagesPerTask = 1,
                SingleProducerConstrained = true,
            });
        }

        protected abstract Task HandleMessage(T msg);
        protected abstract Task<HandleResult> HandleError(BeeMessage<T> msg, Exception ex);



        public void Post(T message)
        {
            IsBusy = true;
            var msg = new BeeMessage<T>(message, 0);
            _mailbox.Post(msg);
        }
        

        [Obsolete("use this only in tests")]
        public async Task WaitEmptyMailBox()
        {
            while (IsBusy)
            {
                await Task.Delay(1);
            }
        }
        
        private async Task Handle(BeeMessage<T> msg)
        {
            IsBusy = true;
            try
            {
                await HandleMessage(msg.Message);
                IsBusy = _mailbox.InputCount > 0;
            }
            catch (Exception e)
            {
                msg.OnError(e);
                var result = await HandleError(msg, e);

                switch (result)
                {
                    case OkHandleResult x: 
                        IsBusy = _mailbox.InputCount > 0;
                        break;
                    
                    case NeedRetry x:
                        _mailbox.Post(msg);
                        break;
                    
                    case NeedRetryWithDelay x:
                        var _ = Task.Delay(x.Delay).ContinueWith(s => _mailbox.Post(msg));
                        break;
                    
                    case NeedRetryWithActorPause x:
                        await Task.Delay(x.Delay);
                        _mailbox.Post(msg);
                        break;
                    
                    default: throw new NotImplementedException("Not implemented handler for result");
                }
            }
        }

    }
}