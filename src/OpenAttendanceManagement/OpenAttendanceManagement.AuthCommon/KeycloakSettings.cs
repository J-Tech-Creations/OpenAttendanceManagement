namespace OpenAttendanceManagement.AuthCommon;

public record KeycloakSettings(string Realm, string ClientId, string ClientSecret, string TokenEndpoint)
{
    private static string GetAuthorityUri(
        string serviceName,
        string realm) =>
        $"https+http://{serviceName}/realms/{realm}";
    private static string GetTokenEndpointUri(
        string ServiceUrl,
        string realm) =>
        $"{ServiceUrl}/realms/{realm}/protocol/openid-connect/token";
    public static KeycloakSettings FromAspire(string ServiceUrl, string Realm, string ClientId, string ClientSecret)
        => new(
            Realm,
            ClientId,
            ClientSecret,
            GetTokenEndpointUri(ServiceUrl, Realm));
}
