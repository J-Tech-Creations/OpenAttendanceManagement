using System.Diagnostics;
using System.Net.Http.Headers;
using System.Web;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.Queries;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Query.QueryModel;

namespace OpenAttendanceManagement.Web.Apis;

public class UserApiClient(HttpClient httpClient, TokenService tokenService, TenantInformation tenantInformation)
{
    public Task<ResultBox<ListQueryResult<OamTenantUsersQuery.Record>>> GetTenantUsers(
        CancellationToken cancellationToken = default) =>
        tokenService.GetTokenAsync()
            .Do(
                success => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", success))
            .Conveyor(() => ResultBox.CheckNull(httpClient.BaseAddress))
            .Conveyor(baseAddress =>
            {
                var uriBuilder = new UriBuilder(baseAddress.Scheme, baseAddress.Host, baseAddress.Port,
                    "/api/query/oamtenantuser/oamtenantusersquery");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["tenantCode.Value"] = tenantInformation.Tenant?.Value?.TenantCode;
                uriBuilder.Query = query.ToString();
                return ResultBox.FromValue(uriBuilder.Uri.ToString());
            })
            .Conveyor(
                uri => ResultBox.WrapTry(() =>
                    httpClient.GetAsync(
                        uri,
                        cancellationToken)))
            .Do(async response => Debug.Print(await response.Content.ReadAsStringAsync()))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<ListQueryResult<OamTenantUsersQuery.Record>>(cancellationToken));
}

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