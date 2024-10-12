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
    public bool IsLoggedIn { get; private set; }

    public Task<ResultBox<string>> GetEmailAsync()
        => ResultBox
            .Start
            .Conveyor(
                _ => ResultBox
                    .CheckNull(httpContextAccessor.HttpContext)
                    .Conveyor(context => ResultBox.CheckNull(context.GetTokenAsync("access_token"))))
            .Log()
            .Conveyor(
                token => ResultBox
                    .FromValue(new JwtSecurityTokenHandler())
                    .Conveyor(
                        handler => ResultBox.CheckNull(
                            handler.ReadJwtToken(token).Claims?.FirstOrDefault(c => c.Type == "email")))
                    .Conveyor(claim => ResultBox.FromValue(claim.Value)));
    public Task<ResultBox<string>> GetIdAsync()
        => ResultBox
            .Start
            .Conveyor(
                _ => ResultBox
                    .CheckNull(httpContextAccessor.HttpContext)
                    .Conveyor(context => ResultBox.CheckNull(context.GetTokenAsync("access_token"))))
            .Conveyor(
                token => ResultBox
                    .FromValue(new JwtSecurityTokenHandler())
                    .Conveyor(
                        handler => ResultBox.CheckNull(
                            handler.ReadJwtToken(token).Claims?.FirstOrDefault(c => c.Type == "sub")))
                    .Conveyor(claim => ResultBox.FromValue(claim.Value)));

    public Task<ResultBox<List<string>>> GetRolesAsync()
        => ResultBox
            .Start
            .Conveyor(
                _ => ResultBox
                    .CheckNull(httpContextAccessor.HttpContext)
                    .Conveyor(context => ResultBox.CheckNull(context.GetTokenAsync("access_token"))))
            .Conveyor(
                token => ResultBox
                    .FromValue(new JwtSecurityTokenHandler())
                    .Conveyor(
                        handler => ResultBox.CheckNull(
                            handler.ReadJwtToken(token).Claims?.FirstOrDefault(c => c.Type == "realm_access")))
                    .Conveyor(claim => ResultBox.FromValue(claim.Value))
                    .Conveyor(
                        value => ResultBox
                            .CheckNull(
                                JsonSerializer.Deserialize<RealmAccess>(
                                    value,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))))
            .Remap(roles => roles.Roles.ToList());

    public Task<ResultBox<UnitValue>> CheckToken() =>
        ResultBox
            .Start
            .Do(() => Roles = new RealmAccess([]))
            // .Conveyor(
            //     () => ResultBox
            //         .CheckNull(
            //             httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "realm_access")))
            .Do(() => IsLoggedIn = httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
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
