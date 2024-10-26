using Account.Grpc.Models;
using Account.Grpc.Protos;
using AutoMapper;

namespace Account.Grpc.Mapper
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountModel, AccountObject>().ReverseMap();
            CreateMap<AccountModelList, AccountObjectList>().ReverseMap();
        }
    }
}
