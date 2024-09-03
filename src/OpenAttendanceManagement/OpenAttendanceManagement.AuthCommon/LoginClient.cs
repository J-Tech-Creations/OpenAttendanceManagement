using System.Net.Http.Json;
using OpenAttendanceManagement.Common.Exceptions;
using ResultBoxes;

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
                .Do(response =>
                    tokenService.SaveTokenAndEmailAsync(response.AccessToken, request.Email ?? string.Empty));

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