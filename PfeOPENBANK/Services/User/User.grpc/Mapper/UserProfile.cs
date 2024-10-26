using AutoMapper;
using User.grpc.Models;
using User.grpc.Protos;

namespace User.grpc.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<UserResponse, UserData>().ReverseMap();
        }
    }
}
