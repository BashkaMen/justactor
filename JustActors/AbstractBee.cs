using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JustActors
{
    public interface IBee {}
    
    public abstract class AbstractBee<T> : IBee
    {
        private readonly MailBox<BeeMessage<T>> _mailbox;
        private readonly Task<Task> _rootTask;
        private readonly ConcurrentBag<TaskCompletionSource<bool>> _waiters;


        private int _messageCounter;
        protected bool IsBusy => _messageCounter != 0;
        protected int QueueCount => _mailbox.Count;
        
        
        public AbstractBee()
        {
            _mailbox = new MailBox<BeeMessage<T>>();
            _waiters = new ConcurrentBag<TaskCompletionSource<bool>>();
            
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
                        OnMessageExit();
                        Console.WriteLine(e);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        protected abstract Task HandleMessage(T msg);
        protected abstract Task<HandleResult> HandleError(BeeMessage<T> msg, Exception ex);



        protected void Post(T message)
        {
            var msg = new BeeMessage<T>(message);
            _mailbox.Post(msg);
            OnMessageEnter();
        }

        protected Task<TResponse> PostAndReply<TResponse>(Func<ReplyChannel<TResponse>, T> msgFabric)
        {
            return _mailbox.PostAndReplyAsync<TResponse>(rc => new BeeMessage<T>(msgFabric(rc)));
        }
        
        protected Task<TResponse?> PostAndReply<TResponse>(Func<ReplyChannel<TResponse>, T> msgFabric, TimeSpan timeout)
        {
            return _mailbox.PostAndReplyAsync<TResponse>(rc => new BeeMessage<T>(msgFabric(rc)), timeout);
        }

        protected void ClearQueue() => _mailbox.Clear();
        

        protected Task WaitEmptyWindow()
        {
            if (!IsBusy) return Task.CompletedTask;
            
            var tsc = new TaskCompletionSource<bool>();
            _waiters.Add(tsc);

            return tsc.Task;
        }
        
        private async Task Handle(BeeMessage<T> msg)
        {
            try
            {
                await HandleMessage(msg.Message);
                OnMessageExit();
            }
            catch (Exception e)
            {
                msg.OnError(e);
                var result = await HandleError(msg, e);

                switch (result)
                {
                    case OkHandleResult x:
                        OnMessageExit();
                        break;
                    
                    case NeedRetry x:
                        _mailbox.Post(msg);
                        break;
                    
                    case NeedRetryWithDelay x:
                        var _ = Task.Delay(x.Delay).ContinueWith(s =>_mailbox.Post(msg));
                        break;
                    
                    
                    default: throw new NotImplementedException("Not implemented handler for result");
                }
            }
        }

        private void OnMessageEnter()
        {
            Interlocked.Increment(ref _messageCounter);
        }

        private void OnMessageExit()
        {
            Interlocked.Decrement(ref _messageCounter);

            if (_messageCounter > 0) return;

            while (_waiters.TryTake(out var tsc))
            {
                tsc.SetResult(true);
            }
        }

    }
}