using System;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public class TimeoutBeeMsg
    {
        public ReplyChannel<Unit> ReplyChannel { get; }
        
        public TimeoutBeeMsg(ReplyChannel<Unit> replyChannel)
        {
            ReplyChannel = replyChannel;
        }
    }
    
    public class TimeoutBee : AbstractBee<TimeoutBeeMsg>
    {

        public Task<Unit> Run() => PostAndReply<Unit>(rc => new TimeoutBeeMsg(rc));
        public Task<Unit?> Run(TimeSpan timeout) => PostAndReply<Unit>(rc => new TimeoutBeeMsg(rc), timeout);
        
        protected override async Task HandleMessage(TimeoutBeeMsg msg)
        {
            await Task.Delay(100);
            msg.ReplyChannel.Reply(Unit.Value);
        }

        protected override Task<HandleResult> HandleError(BeeMessage<TimeoutBeeMsg> msg, Exception ex)
        {
            return HandleResult.OkTask();
        }
    }
}