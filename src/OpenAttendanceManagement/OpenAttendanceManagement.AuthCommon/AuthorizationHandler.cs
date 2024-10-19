using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
namespace OpenAttendanceManagement.AuthCommon;

public class AuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    TokenServiceKeycloakClient tokenServiceKeycloak)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new InvalidOperationException("No HttpContext available from the IHttpContextAccessor!");

        var accessToken = await httpContext.GetTokenAsync("access_token");

        Console.WriteLine(accessToken);

        // jwt token から Roles を取得する
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        var roles = jwtToken
            .Claims
            .Where(claim => claim.Type == "roles")
            .Select(claim => claim.Value)
            .ToList();

        // Check if the token is about to expire in the next 60 seconds
        if (jwtToken.ValidTo < DateTime.UtcNow.AddSeconds(60))
        {
            accessToken = await tokenServiceKeycloak.RefreshTokenAsync();
            Console.WriteLine("New access token: " + accessToken);
        }

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
