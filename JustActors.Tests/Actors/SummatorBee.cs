﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public class SummatorMessage
    {
        public int[] Args { get; }
        public ReplyChannel<int> ReplyChannel { get; }

        public SummatorMessage(int[] args, ReplyChannel<int> replyChannel)
        {
            Args = args;
            ReplyChannel = replyChannel;
        }
    }
    
    public class SummatorBee : AbstractBee<SummatorMessage>
    {
        public Task<int> Sum(params int[] args)
        {
            var rc = new ReplyChannel<int>();
            var msg = new SummatorMessage(args, rc);
            Post(msg);
            
            return rc.GetReply;
        }
        
        
        protected override Task HandleMessage(SummatorMessage msg)
        {
            var sum = msg.Args.Sum();
            msg.ReplyChannel.Reply(sum);

            return Task.CompletedTask;
        }

        protected override Task<HandleResult> HandleError(BeeMessage<SummatorMessage> msg, Exception ex)
        {
            return HandleResult.OkTask();
        }
    }
}