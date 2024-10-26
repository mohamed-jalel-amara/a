using Identity.API.Applications.Models.Entities;
using System;

namespace Identity.API.Utils
{
    public static class UserProviderDetails
    {
        public static UserModel GetUriProviderDetails(HttpRequest request, UserModel user)
        {
            Uri uri = new Uri("http://127.0.0.1:5008");
            if (request != null)
            {
                uri = new Uri(request.Host.ToString());
            }
            user.Provider = uri.ToString();
            user.Provider_id = uri.Host == null || uri.Host == "" ? "localhost" : uri.Host.ToString();
            return user;
        }
    }
}
