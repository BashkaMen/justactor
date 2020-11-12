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
        public static HandleResult Ok() => OkHandleResult.Instance;
        public static Task<HandleResult> OkTask() => OkHandleResult.Task;
        
        public static HandleResult Retry() => NeedRetry.Instance;
        public static Task<HandleResult> RetryTask() => NeedRetry.Task;
        
        
        public static HandleResult Retry(TimeSpan delay) => new NeedRetryWithDelay(delay);
        public static Task<HandleResult> RetryTask(TimeSpan delay) => Task.FromResult(Retry(delay));
        
        protected HandleResult(){}
    }

    internal class OkHandleResult : HandleResult
    {
        internal static readonly OkHandleResult Instance = new OkHandleResult();
        internal static readonly Task<HandleResult> Task = System.Threading.Tasks.Task.FromResult((HandleResult)Instance);
        
        private OkHandleResult(){}
    }

    internal class NeedRetry : HandleResult
    {
        internal static readonly NeedRetry Instance = new NeedRetry();
        internal static readonly Task<HandleResult> Task = System.Threading.Tasks.Task.FromResult((HandleResult)Instance);
        
        private NeedRetry(){}
    }

    internal class NeedRetryWithDelay : HandleResult
    {
        public TimeSpan Delay { get; }

        public NeedRetryWithDelay(TimeSpan delay)
        {
            Delay = delay;
        }
    }
}