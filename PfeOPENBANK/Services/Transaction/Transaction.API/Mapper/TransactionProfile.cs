using Account.API.Models;
using Account.Grpc.Protos;
using AutoMapper;
using EventBus.Message.Events;
using Transaction.API.Features.Commands;
using Transaction.API.Models;

namespace Transaction.API.Mapper
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<TransactionModel, TransactionCmd>().ReverseMap();
            CreateMap<AccountModel, AccountEvent>().ReverseMap();
            CreateMap<AccountEvent, AccountObject>().ReverseMap();
        }
    }
}
