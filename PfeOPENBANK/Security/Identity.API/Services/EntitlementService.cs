using Identity.API.Applications.Data;
using Identity.API.Applications.Dtos;
using Identity.API.Applications.Models.Entities;
using Identity.API.Applications.Models.Responses;
using Identity.API.Services.Interfaces;
using Identity.API.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.Data;
using Twilio.TwiML.Voice;

namespace Identity.API.Services
{
    public class EntitlementService : IEntitlementService
    {
        private readonly UserManager<UserModel>  _userManager;
        private readonly RoleManager<Entitlement> _roleManager;
        private readonly IdentityContext _context;

        public EntitlementService(UserManager<UserModel> userManager, RoleManager<Entitlement> roleManager,
            IdentityContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<EntitlementResponse> AddEntitlementForUserAsync(string userId,EntitlementRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new EntitlementResponse();
            }

            Entitlement role = new Entitlement() { Bank_id = request.Bank_Id, Name = request.Role_name, NormalizedName = request.Role_name };
            IEnumerable<string> roles = new List<string>() { role.Name };

            bool checkIfRoleExist = await this._roleManager.RoleExistsAsync(request.Role_name);
            if (!checkIfRoleExist) { 
                await _roleManager.CreateAsync(role);
            }
            await this._userManager.AddToRolesAsync(user, roles);

            return  GetEntitlementResponseFromModelAsync(await _roleManager.FindByNameAsync(role.Name));
        }

        public async Task<bool> DeleteEntitlementAsync(string userId, string entitlement_id)
        {
            var role = _context.UserRoles.Where( e => e.UserId == userId && e.RoleId == entitlement_id).First();
            if (role == null)
            {
                return false;
            }

            //IdentityResult result = await _roleManager.DeleteAsync(role);
            var result = _context.UserRoles.Remove(role);
            _context.SaveChanges();
            return  true;
        }

        public Entitlements GetAllEntitlements()
        {
            IList<EntitlementResponse> list = new List<EntitlementResponse>();
            var roles = _context.Entitlements.Where(r => !SystemRoles.Roles.Contains(r.Name)).ToList();
            foreach (var item in roles)
            {
                list.Add(GetEntitlementResponseFromModelAsync(item));
            }
            return new Entitlements() { list = list };  
        }

        public async Task<Entitlements> GetEntitlementForUserAsync(string userId)
        {
            IList<EntitlementResponse> list = new List<EntitlementResponse>();
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var item in roles.Where(r => !SystemRoles.Roles.Contains(r)))
                {
                    var role = _context.Entitlements.Where( r => r.Name == item).FirstOrDefault();
                    list.Add(GetEntitlementResponseFromModelAsync(role));
                }
                return new Entitlements() { list = list };
            }
            else
            {
                return new Entitlements();
            }
        }

        private EntitlementResponse GetEntitlementResponseFromModelAsync(Entitlement itemRole)
        {
            if (itemRole == null)
            {
                return new EntitlementResponse();
            }
            EntitlementResponse entitlement = new EntitlementResponse()
            {
                Bank_id = itemRole.Bank_id,
                Entitlement_id = itemRole.Id,
                Role_name = itemRole.Name
            };
            return entitlement;
        }
    }
}
