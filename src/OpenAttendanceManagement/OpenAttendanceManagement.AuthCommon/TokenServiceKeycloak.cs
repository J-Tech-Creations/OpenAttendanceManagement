using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using ResultBoxes;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
namespace OpenAttendanceManagement.AuthCommon;

public class TokenServiceKeycloak(IHttpContextAccessor httpContextAccessor)
{
    private RealmAccess Roles
    {
        get;
        set;
    } = new([]);
    public bool IsSiteAdmin => Roles.IsInRole("SiteAdmin");
    public Task<ResultBox<UnitValue>> CheckToken() =>
        ResultBox
            .Start
            .Do(() => Roles = new RealmAccess([]))
            // .Conveyor(
            //     () => ResultBox
            //         .CheckNull(
            //             httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "realm_access")))
            .Conveyor(
                _ => ResultBox
                    .CheckNull(httpContextAccessor.HttpContext)
                    .Conveyor(context => ResultBox.CheckNull(context.GetTokenAsync("access_token"))))
            .Conveyor(
                token => ResultBox
                    .FromValue(new JwtSecurityTokenHandler())
                    .Conveyor(
                        handler => ResultBox.CheckNull(
                            handler.ReadJwtToken(token).Claims?.FirstOrDefault(c => c.Type == "realm_access"))))
            .Conveyor(
                claim => ResultBox.CheckNull(
                    JsonSerializer.Deserialize<RealmAccess>(
                        claim.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })))
            .Do(
                access => Roles = access)
            .Conveyor(_ => ResultBox.UnitValue);
}
