namespace Identity.API.Applications.Models.Responses
{
    public class UserResponseList
    {
        public IList<UserResponse> Users { get; set; } = new List<UserResponse>();
    }
}
