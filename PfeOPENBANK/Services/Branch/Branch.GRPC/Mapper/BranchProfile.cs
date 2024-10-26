using AutoMapper;
using Branch.GRPC.Models;
using Branch.GRPC.Protos;

namespace Branch.GRPC.Mapper
{
    public class BranchProfile : Profile
    {
        public BranchProfile() {
            CreateMap<BranchModel, BranchResponse>().ReverseMap();
        }
    }
}
