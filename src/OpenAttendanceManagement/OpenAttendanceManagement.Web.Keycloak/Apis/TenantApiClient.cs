using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Web.Keycloak.Apis;

public class TenantApiClient(HttpClient httpClient, TenantInformation tenantInformation)
{
    public Task<ResultBox<TenantInformation>> GetMyTenants(
        CancellationToken cancellationToken = default) =>
        ResultBox
            .Start
            .Conveyor(
                _ => ResultBox.CheckNullWrapTry(
                    async () =>
                        await httpClient.GetFromJsonAsync<ListQueryResult<BelongingTenantQuery.Record>>(
                            "/user/tenants",
                            cancellationToken)))
            .Do(
                records =>
                {
                    tenantInformation.Tenants = records.Items.ToList();
                    if (tenantInformation.Tenants.Any() &&
                        tenantInformation.Tenants.All(t => t.TenantId != tenantInformation.Tenant?.Value?.TenantId))
                        tenantInformation.Tenant = tenantInformation.Tenants.First();
                })
            .Conveyor(() => ResultBox.FromValue(tenantInformation));

    public Task<ResultBox<UnitValue>> AddUserToTenant(
        OamTenantCreateUser command,
        CancellationToken cancellationToken = default) =>
        ResultBox
            .Start
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/oamtenantcreateuser",
                        command,
                        cancellationToken)))
            .Conveyor(
                response =>
                    ResultBox.FromValue(response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken)))
            .Remap(_ => UnitValue.Unit);

    public Task<ResultBox<UnitValue>> AddUserAcceptInviteOnTenant(
        OamTenantUserAcceptInvite command,
        CancellationToken cancellationToken = default) =>
        ResultBox
            .Start
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/oamtenantuseracceptinvite",
                        command,
                        cancellationToken)))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken))
            .Remap(_ => UnitValue.Unit);
}
