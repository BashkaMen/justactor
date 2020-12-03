using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JustActors
{
    public interface IBee {}
    
    public abstract class AbstractBee<T> : IBee
    {
        private readonly BufferBlock<BeeMessage<T>> _mailbox;
        private readonly Task<Task> _rootTask;


        private int _messageCounter;
        public bool IsBusy => _messageCounter != 0;
        
        
        public AbstractBee()
        {
            _mailbox = new BufferBlock<BeeMessage<T>>();
            
            _rootTask = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var msg = await _mailbox.ReceiveAsync();

                    try
                    {
                        await Handle(msg);
                    }
                    catch (Exception e)
                    {
                        Interlocked.Decrement(ref _messageCounter);
                        Console.WriteLine(e);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        protected abstract Task HandleMessage(T msg);
        protected abstract Task<HandleResult> HandleError(BeeMessage<T> msg, Exception ex);



        public void Post(T message)
        {
            var msg = new BeeMessage<T>(message, 0);
            Interlocked.Increment(ref _messageCounter);
            _mailbox.Post(msg);
        }

        protected void ClearQueue() => _mailbox.TryReceiveAll(out var _);
        

        [Obsolete("use this only in tests")]
        public async Task WaitEmptyMailBox()
        {
            while (IsBusy)
            {
                await Task.Delay(10);
            }
        }
        
        private async Task Handle(BeeMessage<T> msg)
        {
            try
            {
                await HandleMessage(msg.Message);
                Interlocked.Decrement(ref _messageCounter);
            }
            catch (Exception e)
            {
                msg.OnError(e);
                var result = await HandleError(msg, e);

                switch (result)
                {
                    case OkHandleResult x: 
                        Interlocked.Decrement(ref _messageCounter);
                        break;
                    
                    case NeedRetry x:
                        _mailbox.Post(msg);
                        break;
                    
                    case NeedRetryWithDelay x:
                        var _ = Task.Delay(x.Delay).ContinueWith(s =>_mailbox.Post(msg));
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