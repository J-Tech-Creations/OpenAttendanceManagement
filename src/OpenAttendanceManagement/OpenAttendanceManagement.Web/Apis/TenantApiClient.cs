using System.Net.Http.Headers;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using ResultBoxes;
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
}