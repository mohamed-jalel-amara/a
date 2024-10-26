using AutoMapper;
using Bank.grpc.Models;
using Bank.grpc.Protos;

namespace Bank.grpc.Mapper
{
    public class BankProfile : Profile
    {
        public BankProfile()
        {
            CreateMap<BankObject, BankModel>().ReverseMap();
        }
    }
}
