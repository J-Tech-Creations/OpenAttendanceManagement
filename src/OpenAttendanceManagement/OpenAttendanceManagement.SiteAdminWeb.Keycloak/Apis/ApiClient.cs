using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.SiteAdminWeb.Keycloak.Apis;

public class ApiClient(HttpClient httpClient)
{
    public Task<ResultBox<ListQueryResult<SimpleTenantQuery.Record>>> GetTenants(
        CancellationToken cancellationToken = default) =>
        ResultBox.Start
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.GetFromJsonAsync<ListQueryResult<SimpleTenantQuery.Record>>(
                        "/api/query/oamtenant/simpletenantquery",
                        cancellationToken)));

    public Task<ResultBox<UnitValue>> AddTenant(
        CreateOamTenant command,
        CancellationToken cancellationToken = default) =>
        ResultBox
            .Start
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
        ResultBox
            .Start
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
        ResultBox
            .Start
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
        ResultBox
            .Start
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
        ResultBox
            .Start
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
