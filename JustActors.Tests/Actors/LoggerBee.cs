using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public interface ILoggerMessage{}

    public class LogMessage : ILoggerMessage
    {
        public string Text { get; }

        public LogMessage(string text)
        {
            Text = text;
        }
    }

    public class FlushMessage : ILoggerMessage { }
    
    
    public class LoggerBee : AbstractBee<ILoggerMessage>
    {
        private const string Path = "log.txt";
        private readonly List<string> _log = new List<string>();
        
        // simple helper
        public void LogMessage(string message) => Post(new LogMessage(message));
        public void Flush() => Post(new FlushMessage());
        
        protected override async Task HandleMessage(ILoggerMessage msg)
        {
            switch (msg)
            {
                case LogMessage m:
                    _log.Add(m.Text);
                    break;
                
                case FlushMessage _:
                    await File.WriteAllLinesAsync(Path, _log);
                    _log.Clear();
                    break;
            }
        }

        protected override Task HandleError(BeeMessage<ILoggerMessage> msg, Exception ex)
        {
            if (msg.Attemp > 3) return Task.CompletedTask;

            Post(msg.Message); // retry

            return Task.CompletedTask;
        }
    }
}