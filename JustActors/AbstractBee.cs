using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JustActors
{
    public interface IBee {}
    
    public abstract class AbstractBee<T> : IBee
    {
        private readonly BufferBlock<BeeMessage<T>> _mailbox;

        protected bool IsBusy { get; private set; }

        protected AbstractBee()
        {
            _mailbox = new BufferBlock<BeeMessage<T>>();
            
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var msg = await _mailbox.ReceiveAsync();
                        await Handle(msg);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
        }

        protected abstract Task HandleMessage(T msg);
        protected abstract Task<HandleResult> HandleError(BeeMessage<T> msg, Exception ex);
        
        public void Post(T message)
        {
            var msg = new BeeMessage<T>(message, 0);
            IsBusy = true;
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
            try
            {
                await HandleMessage(msg.Message);
                IsBusy = _mailbox.Count > 0;
            }
            catch (Exception e)
            {
                msg.OnError(e);
                var result = await HandleError(msg, e);

                switch (result)
                {
                    case OkHandleResult x:
                        IsBusy = _mailbox.Count > 0;
                        break;
                    case NeedRetry x:
                        _mailbox.Post(msg);
                        break;
                    case NeedRetryWithDelay x:
                        var _ = Task.Delay(x.Delay).ContinueWith(s => _mailbox.Post(msg));
                        break;
                    
                    default: throw new NotImplementedException("Not implemented handler for result");
                }

            }
        }

    }
}