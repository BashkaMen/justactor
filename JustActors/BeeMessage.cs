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
}