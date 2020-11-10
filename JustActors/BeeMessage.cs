using System;
using System.Threading.Tasks;

namespace JustActors
{
    public class BeeMessage<T>
    {
        public T Message { get; }
        public int Attemp { get; private set; }
        public Exception LastError { get; private set; }

        
        public BeeMessage(T message, int attemp)
        {
            Message = message;
            Attemp = attemp;
        }


        public void OnError(Exception ex)
        {
            LastError = ex;
            Attemp++;
        }
    }

    
    public class HandleResult
    {
        public static HandleResult Ok() => new OkHandleResult();
        public static Task<HandleResult> OkTask() => Task.FromResult(Ok());
        
        public static HandleResult Retry() => new NeedRetry();
        public static Task<HandleResult> RetryTask() => Task.FromResult(Retry());
        
        
        public static HandleResult Retry(TimeSpan delay) => new NeedRetryWithDelay(delay);
        public static Task<HandleResult> RetryTask(TimeSpan delay) => Task.FromResult(Retry(delay));
        
        protected HandleResult(){}
    }
    
    
    internal class OkHandleResult : HandleResult {}
    internal class NeedRetry : HandleResult {}

    internal class NeedRetryWithDelay : HandleResult
    {
        public TimeSpan Delay { get; }

        public NeedRetryWithDelay(TimeSpan delay)
        {
            Delay = delay;
        }
    }

    
}