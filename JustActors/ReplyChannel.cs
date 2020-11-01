using System.Threading.Tasks;

namespace JustActors
{
    public class ReplyChannel<T>
    {
        private readonly TaskCompletionSource<T> _tsc;

        public Task<T> GetReply => _tsc.Task;
        
        public ReplyChannel()
        {
            _tsc = new TaskCompletionSource<T>();
        }

        public void Reply(T reply)
        {
            _tsc.SetResult(reply);
        }
    }
}