using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ResultBoxes;

namespace OpenAttendanceManagement.Common;

public class OamUserManager(
    UserManager<IdentityUser> userManager,
    IHttpContextAccessor httpContextAccessor)
{
    public Task<ResultBox<IdentityUser>> GetUser() =>
        ResultBox.CheckNull(
                httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier))
            .Conveyor(claim => ResultBox.CheckNull(userManager.FindByIdAsync(claim.Value)));

    public Task<ResultBox<string>> GetUserEmail() =>
        GetUser().Conveyor(user => ResultBox.CheckNull(user.Email));
}