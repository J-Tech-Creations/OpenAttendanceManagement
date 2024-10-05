using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
namespace OpenAttendanceManagement.AuthCommon;

public static class LoginLogoutEndpointRouteBuilderExtensions
{

    public static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("authentication");

        group
            .MapGet("/login", () => TypedResults.Challenge(new AuthenticationProperties { RedirectUri = "/" }))
            .AllowAnonymous();

        group.MapPost(
            "/logout",
            () => TypedResults.SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

        return group;
    }
}
