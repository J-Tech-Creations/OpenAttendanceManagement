using OpenAttendanceManagement.Web.Exceptions;
using OpenAttendanceManagement.Web.Tokens;
using ResultBoxes;
namespace OpenAttendanceManagement.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(
        int maxItems = 10,
        CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>(
            "/weatherforecast",
            cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
}
public class LoginClient(HttpClient httpClient, TokenService tokenService)
{
    public async Task<ResultBox<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
        => await ResultBox
            .WrapTry(() => httpClient.PostAsJsonAsync("login", request, cancellationToken))
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
