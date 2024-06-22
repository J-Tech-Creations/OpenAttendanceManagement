using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common.Exceptions;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Query.QueryModel;
using System.Net.Http.Headers;
namespace OpenAttendanceManagement.SiteAdminWeb.Apis;

public class ApiClient(HttpClient httpClient, TokenService tokenService)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(
        int maxItems = 10,
        CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success));

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
    public Task<ResultBox<ListQueryResult<SimpleTenantQuery.Record>>> GetTenants(
        CancellationToken cancellationToken = default) =>
        tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success))
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.GetFromJsonAsync<ListQueryResult<SimpleTenantQuery.Record>>(
                        "/simpletenantquery/simpletenantquery",
                        cancellationToken)));

    public Task<ResultBox<UnitValue>> AddTenant(
        CreateOamTenant command,
        CancellationToken cancellationToken = default) =>
        tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success))
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/createoamtenant",
                        command,
                        cancellationToken)))
            .DoWrapTry(response => response.EnsureSuccessStatusCode())
            .Conveyor(
                async response => ResultBox.CheckNull(
                    await response.Content.ReadFromJsonAsync<CommandExecutorResponse>(
                        cancellationToken),
                    new TenantAddException("ログインに失敗しました。")))
            .Remap(_ => UnitValue.Unit);
}
