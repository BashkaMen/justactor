using System;
using System.Threading.Tasks;

namespace JustActors.Tests.Actors
{
    public interface IPushService
    {
        Task SendNotification(string userId, string textMessage);
    }
    
    public class NotificationMessage
    {
        public string UserId { get; }
        public string Message { get; }

        public NotificationMessage(string userId, string message)
        {
            UserId = userId;
            Message = message;
        }
    }
    
    public class NotificationBee : AbstractBee<NotificationMessage>
    {
        private readonly IPushService _pushService;

        public NotificationBee(IPushService pushService)
        {
            _pushService = pushService;
        }
        
        protected override async Task HandleMessage(NotificationMessage msg)
        {
            await _pushService.SendNotification(msg.UserId, msg.Message);
        }

        protected override Task<HandleResult> HandleError(BeeMessage<NotificationMessage> msg, Exception ex)
        {
            return HandleResult.RetryTask(TimeSpan.FromSeconds(3));
        }
    }
}