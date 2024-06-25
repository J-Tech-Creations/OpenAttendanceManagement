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
                        "/oamtenant/simpletenantquery",
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

    public Task<ResultBox<UnitValue>> ChangeTenantName(
        ChangeOamTenantName command,
        CancellationToken cancellationToken = default) =>
        tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success))
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/changeoamtenantname",
                        command,
                        cancellationToken)))
            .DoWrapTry(response => response.EnsureSuccessStatusCode())
            .Conveyor(
                async response => ResultBox.CheckNull(
                    await response.Content.ReadFromJsonAsync<CommandExecutorResponse>(
                        cancellationToken),
                    new TenantChangeNameException("ログインに失敗しました。")))
            .Remap(_ => UnitValue.Unit);
    public Task<ResultBox<UnitValue>> DeleteTenant(
        DeleteOamTenant command,
        CancellationToken cancellationToken = default) =>
        tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success))
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/deleteoamtenant",
                        command,
                        cancellationToken)))
            .DoWrapTry(response => response.EnsureSuccessStatusCode())
            .Conveyor(
                async response => ResultBox.CheckNull(
                    await response.Content.ReadFromJsonAsync<CommandExecutorResponse>(
                        cancellationToken),
                    new TenantDeleteException("テナントの削除に失敗しました。")))
            .Remap(_ => UnitValue.Unit);

    // /api/command/oamtenant/oattenantaddauthidentity

    public Task<ResultBox<UnitValue>> AddTenantAdmin(
        OamTenantAddAuthIdentity command,
        CancellationToken cancellationToken = default) =>
        tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success))
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/oamtenantaddauthidentity",
                        command,
                        cancellationToken)))
            .DoWrapTry(response => response.EnsureSuccessStatusCode())
            .Conveyor(
                async response => ResultBox.CheckNull(
                    await response.Content.ReadFromJsonAsync<CommandExecutorResponse>(
                        cancellationToken),
                    new TenantAddAdminException("管理者の追加に失敗しました。")))
            .Remap(_ => UnitValue.Unit);
}
