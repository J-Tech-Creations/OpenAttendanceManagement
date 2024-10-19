using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
namespace OpenAttendanceManagement.AuthCommon;

public class TokenServiceKeycloakClient(
    IHttpContextAccessor httpContextAccessor,
    HttpClient httpClient,
    KeycloakSettings keycloakSettings)
{
    public async Task<string> RefreshTokenAsync()
    {
        var refreshToken = await httpContextAccessor.HttpContext?.GetTokenAsync("refresh_token");
        if (refreshToken == null)
        {
            throw new InvalidOperationException("No refresh token available.");
        }

        var tokenEndpoint = keycloakSettings.TokenEndpoint;
        var clientId = keycloakSettings.ClientId;
        var clientSecret = keycloakSettings.ClientSecret;

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken }
                })
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();
        var serialized = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
        var newAccessToken = serialized["access_token"].ToString();
        var newRefreshToken = serialized["refresh_token"].ToString();

        // 現在の認証情報を取得
        var authenticateResult
            = await httpContextAccessor.HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

        if (authenticateResult.Succeeded && authenticateResult.Principal != null)
        {
            // 新しいトークン情報をプロパティに設定
            var newProperties = authenticateResult.Properties;
            newProperties.UpdateTokenValue("access_token", newAccessToken);
            newProperties.UpdateTokenValue("refresh_token", newRefreshToken);

            // 新しいクレーム情報で再サインイン
            var claimsPrincipal = authenticateResult.Principal;

            await httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                newProperties);
        }
        return newAccessToken;
    }
}
