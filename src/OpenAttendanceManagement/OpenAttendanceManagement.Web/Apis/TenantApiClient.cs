using System.Net.Http.Headers;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Query.QueryModel;

namespace OpenAttendanceManagement.Web.Apis;

public class TenantApiClient(HttpClient httpClient, TokenService tokenService, TenantInformation tenantInformation)
{
    public Task<ResultBox<TenantInformation>> GetMyTenants(
        CancellationToken cancellationToken = default) =>
        tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success))
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.GetFromJsonAsync<ListQueryResult<BelongingTenantQuery.Record>>(
                        "/user/tenants",
                        cancellationToken)))
            .Do(records =>
            {
                tenantInformation.Tenants = records.Items.ToList();
                if (tenantInformation.Tenants.Any() &&
                    tenantInformation.Tenants.All(t => t.TenantId != tenantInformation.Tenant?.Value?.TenantId))
                    tenantInformation.Tenant = tenantInformation.Tenants.First();
            }).Conveyor(() => ResultBox.FromValue(tenantInformation));

    public Task<ResultBox<UnitValue>> AddUserToTenant(
        OamTenantCreateUser command,
        CancellationToken cancellationToken = default) =>
        tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success))
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/api/command/oamtenant/oamtenantcreateuser",
                        command,
                        cancellationToken)))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken))
            .Remap(_ => UnitValue.Unit);
}