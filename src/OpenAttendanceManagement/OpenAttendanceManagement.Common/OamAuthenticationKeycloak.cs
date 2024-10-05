using ResultBoxes;
using System.Security.Claims;
namespace OpenAttendanceManagement.Common;

public class OamAuthenticationKeycloak(IHttpContextAccessor contextAccessor) : IOamAuthentication
{
    public Task<ResultBox<OatLoginUser>> GetOatLoginUser()
        => ResultBox
            .Start
            .Conveyor(
                _ => ResultBox.CheckNullWrapTry(() => contextAccessor.HttpContext?.User))
            .Conveyor(
                user => ResultBox.CheckNull(
                    user.Claims.FirstOrDefault(
                        c => c.Type == ClaimTypes.NameIdentifier)))
            .Conveyor(_ => ResultBox.FromValue(new OatLoginUser("keycloak", new List<string> { "keycloak" })))
            .ToTask();
}
