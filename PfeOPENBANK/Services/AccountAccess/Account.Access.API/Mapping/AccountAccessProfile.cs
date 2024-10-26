using Account.Access.API.Models;
using AutoMapper;
using EventBus.Message.Events;

namespace Account.Access.API.Mapping
{
    public class AccountAccessProfile : Profile
    {
        public AccountAccessProfile() 
        {
            CreateMap<AccountAccess, AccountAccessEvent>().ReverseMap();
        }
    }
}
