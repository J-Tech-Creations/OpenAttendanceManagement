using Microsoft.AspNetCore.Identity;
using ResultBoxes;
using System.Security.Claims;
namespace OpenAttendanceManagement.Common;

public class OamUserManager(
    UserManager<IdentityUser> userManager,
    IHttpContextAccessor httpContextAccessor) : IOamUserManager
{

    public Task<ResultBox<string>> GetExecutingUserEmail() =>
        GetExecutingUser().Conveyor(user => ResultBox.CheckNull(user.Email));

    public async Task<ResultBox<OptionalValue<string>>> GetUserIdFromEmail(string email) => ResultBox
        .FromValue(OptionalValue.FromNullableValue(await userManager.FindByEmailAsync(email)))
        .Remap(user => user.HasValue ? user.GetValue().Id : OptionalValue<string>.Null);
    public Task<ResultBox<IdentityUser>> GetExecutingUser() =>
        ResultBox
            .CheckNull(
                httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier))
            .Conveyor(claim => ResultBox.CheckNull(userManager.FindByIdAsync(claim.Value)));
}
