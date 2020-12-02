using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JustActors
{
    public interface IBee {}
    
    public abstract class AbstractBee<T> : IBee
    {
        private readonly List<BeeMessage<T>> _delayedMessages;
        private readonly BufferBlock<BeeMessage<T>> _mailbox;
        private readonly Task<Task> _rootTask;

        private bool _inProcess;
        
        public bool IsBusy => _inProcess || _delayedMessages.Count + _mailbox.Count > 0;
        
        public AbstractBee()
        {
            _mailbox = new BufferBlock<BeeMessage<T>>();
            _delayedMessages = new List<BeeMessage<T>>();

            _rootTask = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    _inProcess = false;
                    var msg = await _mailbox.ReceiveAsync();

                    try
                    {
                        _inProcess = true;
                        await Handle(msg);
                    }
                    catch (Exception e)
                    {
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
            _mailbox.Post(msg);
        }

        protected void ClearQueue() => _mailbox.TryReceiveAll(out var _);
        

        [Obsolete("use this only in tests")]
        public async Task WaitEmptyMailBox()
        {
            while (IsBusy)
            {
                await Task.Delay(30);
            }
        }
        
        private async Task Handle(BeeMessage<T> msg)
        {
            try
            {
                await HandleMessage(msg.Message);
            }
            catch (Exception e)
            {
                msg.OnError(e);
                var result = await HandleError(msg, e);

                switch (result)
                {
                    case OkHandleResult x: 
                        break;
                    
                    case NeedRetry x:
                        _mailbox.Post(msg);
                        break;
                    
                    case NeedRetryWithDelay x:
                        _delayedMessages.Add(msg);
                        
                        var _ = Task.Delay(x.Delay).ContinueWith(s =>
                        {
                            _mailbox.Post(msg);
                            _delayedMessages.Remove(msg);
                        });
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