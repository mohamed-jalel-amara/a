using Account.API.Models;
using Account.API.Models.Requests;
using AutoMapper;
using EventBus.Message.Events;
using View.grpc.Protos;

namespace Account.API.Mapper
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountModel, AccountEvent>().ReverseMap();
            CreateMap<ViewModelList, ViewObjectList>().ReverseMap();
            CreateMap<ViewModel,  ViewObject>().ReverseMap();
        }
    }
}
