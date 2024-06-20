using OpenAttendanceManagement.Common.Exceptions;
using ResultBoxes;
using System.Net.Http.Json;
namespace OpenAttendanceManagement.AuthCommon;

public class LoginClient(HttpClient httpClient, TokenService tokenService)
{
    public Task<ResultBox<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
        =>
            ResultBox.WrapTry(() => httpClient.PostAsJsonAsync("login", request, cancellationToken))
                .DoWrapTry(response => response.EnsureSuccessStatusCode())
                .Conveyor(
                    async response => ResultBox.CheckNull(
                        await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken),
                        new LoginException("ログインに失敗しました。")))
                .Do(response => tokenService.SaveTokenAsync(response.AccessToken));



    public record LoginRequest
    {
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public bool UseCookies { get; set; } = false;
    }
    public record LoginResponse(
        string TokenType,
        string AccessToken,
        int ExpiresIn,
        string RefreshToken);
}
