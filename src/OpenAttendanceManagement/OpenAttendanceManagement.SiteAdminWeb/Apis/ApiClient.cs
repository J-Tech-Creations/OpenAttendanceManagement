using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Query.QueryModel;

namespace OpenAttendanceManagement.SiteAdminWeb.Apis;

public class ApiClient(HttpClient httpClient, TokenService tokenService)
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

    public Task<ResultBox<ListQueryResult<SimpleTenantQuery.Record>>> GetTenants(
        CancellationToken cancellationToken = default) =>
        tokenService.SetTokenToHeader(httpClient)
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.GetFromJsonAsync<ListQueryResult<SimpleTenantQuery.Record>>(
                        "/api/query/oamtenant/simpletenantquery",
                        cancellationToken)));

    public Task<ResultBox<UnitValue>> AddTenant(
        CreateOamTenant command,
        CancellationToken cancellationToken = default) =>
        tokenService.SetTokenToHeader(httpClient)
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/createoamtenant",
                        command,
                        cancellationToken)))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken))
            .Remap(_ => UnitValue.Unit);

    public Task<ResultBox<UnitValue>> ChangeTenantName(
        ChangeOamTenantName command,
        CancellationToken cancellationToken = default) =>
        tokenService.SetTokenToHeader(httpClient)
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/changeoamtenantname",
                        command,
                        cancellationToken)))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken))
            .Remap(_ => UnitValue.Unit);

    public Task<ResultBox<UnitValue>> DeleteTenant(
        DeleteOamTenant command,
        CancellationToken cancellationToken = default) =>
        tokenService.SetTokenToHeader(httpClient)
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/deleteoamtenant",
                        command,
                        cancellationToken)))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken))
            .Remap(_ => UnitValue.Unit);

    // /api/command/oamtenant/oattenantaddauthidentity

    public Task<ResultBox<UnitValue>> AddTenantAdmin(
        OamTenantAddAuthIdentity command,
        CancellationToken cancellationToken = default) =>
        tokenService.SetTokenToHeader(httpClient)
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/oamtenantaddauthidentity",
                        command,
                        cancellationToken)))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken))
            .Remap(_ => UnitValue.Unit);

    public Task<ResultBox<UnitValue>> RemoveTenantAdmin(
        OamTenantRemoveAuthIdentity command,
        CancellationToken cancellationToken = default) =>
        tokenService.SetTokenToHeader(httpClient)
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/oamtenantremoveauthidentity",
                        command,
                        cancellationToken)))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken))
            .Remap(_ => UnitValue.Unit);
}