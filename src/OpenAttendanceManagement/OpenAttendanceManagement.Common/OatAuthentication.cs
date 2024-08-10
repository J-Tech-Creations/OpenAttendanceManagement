using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ResultBoxes;

namespace OpenAttendanceManagement.Common;

public class OatAuthentication(
    UserManager<IdentityUser> userManager,
    IHttpContextAccessor contextAccessor) : IOatAuthentication
{
    public Task<ResultBox<OatLoginUser>> GetOatLoginUser()
        => ResultBox.Start.Conveyor(
                _ => ResultBox.CheckNullWrapTry(() => contextAccessor.HttpContext?.User))
            .Conveyor(
                user => ResultBox.CheckNull(
                    user.Claims.FirstOrDefault(
                        c => c.Type == ClaimTypes.NameIdentifier)))
            .Conveyor(
                async userId => ResultBox.CheckNull(await userManager.FindByIdAsync(userId.Value)))
            .Combine(user => ResultBox.FromValue(userManager.GetRolesAsync(user)))
            .Conveyor(
                (user, roles) => user.Email is not null
                    ? ResultBox.FromValue(new OatLoginUser(user.Email, roles.ToList()))
                    : new ApplicationException("User not found."));
}