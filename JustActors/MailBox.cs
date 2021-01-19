using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JustActors
{
    public class MailBox<T>
    {
        private readonly BufferBlock<T> _mailbox;

        public bool IsEmpty => _mailbox.Count == 0;
        public int Count => _mailbox.Count;
        
        public MailBox()
        {
            _mailbox = new BufferBlock<T>();
        }

        public void Post(T msg) => _mailbox.Post(msg);
        public void Clear() => _mailbox.TryReceiveAll(out _);

        public Task<T> ReceiveAsync() => _mailbox.ReceiveAsync();

        public Task<TResponse> PostAndReplyAsync<TResponse>(Func<ReplyChannel<TResponse>, T> msgFabric)
        {
            var rc = new ReplyChannel<TResponse>();
            var msg = msgFabric(rc);

            Post(msg);
            
            return rc.GetReply;
        }

        public async Task<TResponse?> PostAndReplyAsync<TResponse>(Func<ReplyChannel<TResponse>, T> msgFabric, TimeSpan timeout)
        {
            var postTask = PostAndReplyAsync(msgFabric);
            var delayTask = Task.Delay(timeout);

            var resTask = await Task.WhenAny(postTask, delayTask);

            return resTask == delayTask ? default : postTask.Result;
        }
    }
}