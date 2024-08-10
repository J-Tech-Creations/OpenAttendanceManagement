using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ResultBoxes;

namespace OpenAttendanceManagement.Common;

public class OamUserManager(
    UserManager<IdentityUser> userManager,
    IHttpContextAccessor httpContextAccessor) : IOamUserManager
{
    public Task<ResultBox<IdentityUser>> GetExecutingUser() =>
        ResultBox.CheckNull(
                httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier))
            .Conveyor(claim => ResultBox.CheckNull(userManager.FindByIdAsync(claim.Value)));

    public Task<ResultBox<string>> GetExecutingUserEmail() =>
        GetExecutingUser().Conveyor(user => ResultBox.CheckNull(user.Email));

    public Task<ResultBox<string>> GetUserIdFromEmail(string email) => ResultBox
        .CheckNull(userManager.FindByEmailAsync(email))
        .Remap(user => user.Id);
}