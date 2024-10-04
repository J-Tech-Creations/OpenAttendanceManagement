using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sekiban.Core.Command;
using Sekiban.Web.Authorizations;
using Sekiban.Web.Authorizations.Definitions;
using System.Security.Claims;
using System.Text.Json;
namespace OpenAttendanceManagement.AuthCommon;

public enum OamRoles
{
    SiteAdmin,
    User
}
public class
    AuthorizeDefinitionCollectionWithKeycloak : IAuthorizeDefinitionCollection
{

    public static AuthorizeDefinitionCollection AllowAllIfLoggedIn =>
        new(new AllowIfLoggedIn<AllMethod>());
    public static AuthorizeDefinitionCollection AllowAll => new(new Allow<AllMethod>());
    public AuthorizeDefinitionCollectionWithKeycloak(
        IEnumerable<IAuthorizeDefinition> collection) => Collection = collection;

    public AuthorizeDefinitionCollectionWithKeycloak(
        params IAuthorizeDefinition[] definitions) => Collection = definitions;

    public IEnumerable<IAuthorizeDefinition> Collection { get; set; }
    public async Task<AuthorizeResultType> CheckAuthorization(
        AuthorizeMethodType authorizeMethodType,
        ControllerBase controller,
        Type aggregateType,
        Type? commandType,
        ICommandCommon? command,
        HttpContext httpContext,
        IServiceProvider serviceProvider)
    {

        foreach (var definition in Collection)
        {
            var result = await definition.Check(
                authorizeMethodType,
                aggregateType,
                commandType,
                async roles =>
                {
                    var isInRole = false;

                    var userIdClaim =
                        httpContext.User.Claims.FirstOrDefault(
                            c => c.Type == ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)

                    {
                        return isInRole;
                    }

                    var realmAccessClaim =
                        httpContext.User.Claims.FirstOrDefault(
                            c => c.Type == "realm_access");
                    if (realmAccessClaim != null)
                    {
                        var realmRoles = JsonSerializer.Deserialize<RealmAccess>(
                            realmAccessClaim?.Value ?? "",
                            new JsonSerializerOptions
                                { PropertyNameCaseInsensitive = true });
                        if (realmRoles != null)
                        {
                            foreach (var role in roles)
                            {
                                if (realmRoles.IsInRole(role))
                                {
                                    isInRole = true;
                                }
                            }
                        }
                    }
                    return isInRole;
                },
                httpContext,
                serviceProvider);
            if (result == AuthorizeResultType.Allowed || result == AuthorizeResultType.Denied)
            {
                return result;
            }
        }

        return AuthorizeResultType.Passed;
    }

    public void Add(IAuthorizeDefinition definition)
    {
        Collection = new List<IAuthorizeDefinition>(Collection) { definition };
    }

    private record RealmAccess(string[] Roles)
    {
        public bool IsInRole(string role) => Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
    }
}
