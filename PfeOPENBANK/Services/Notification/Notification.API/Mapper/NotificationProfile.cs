using AutoMapper;
using EventBus.Message.Events;
using Notification.API.Models;

namespace Notification.API.Mapper
{
    public class NotificationProfile  : Profile
    {
        public NotificationProfile()
        {
            CreateMap<IdentityMessage, SendSmsEvent>().ReverseMap();
            CreateMap<Email, SendEmailEvent>().ReverseMap();
        }
    }
}
