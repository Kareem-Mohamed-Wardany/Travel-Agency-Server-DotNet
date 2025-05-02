using Microsoft.AspNetCore.Identity;

namespace Travel_Agency_Server.Services.Auth
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(IdentityUser user, UserManager<IdentityUser> userManager);
    }
}
