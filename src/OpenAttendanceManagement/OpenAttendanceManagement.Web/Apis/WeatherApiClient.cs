using OpenAttendanceManagement.AuthCommon;

namespace OpenAttendanceManagement.Web.Apis;

public class WeatherApiClient(HttpClient httpClient, TokenService tokenService)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(
        int maxItems = 10,
        CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await tokenService.SetTokenToHeader(httpClient);

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>(
                           "/weatherforecast",
                           cancellationToken))
        {
            if (forecasts?.Count >= maxItems) break;
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
}