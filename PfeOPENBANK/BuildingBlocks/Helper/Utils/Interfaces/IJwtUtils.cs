using Helper.Utils.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Helper.Utils.Interfaces
{
    public interface IJwtUtils
    {
        LoggedUser GetLoggedUser(string token);
        string ValidateToken(string token);
    }
}
