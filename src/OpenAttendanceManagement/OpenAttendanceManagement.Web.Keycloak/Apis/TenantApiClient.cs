using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Queries;
using OpenAttendanceManagement.Domain.Usecases;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Query.QueryModel;
using System.Diagnostics;
using System.Web;
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

    public Task<ResultBox<ListQueryResult<OamTermTenantsListQuery.Record>>> GetTenantTermList(
        CancellationToken cancellationToken = default) =>
        ResultBox
            .Start
            .Conveyor(() => ResultBox.CheckNull(httpClient.BaseAddress))
            .Conveyor(
                baseAddress =>
                {
                    var uriBuilder = new UriBuilder(
                        baseAddress.Scheme,
                        baseAddress.Host,
                        baseAddress.Port,
                        "/api/query/oamtermtenant/oamtermtenantslistquery");
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["tenantCode.Value"] = tenantInformation.Tenant?.Value?.TenantCode;
                    query["tenantId.Value"] = tenantInformation.Tenant?.Value?.TenantId.ToString() ?? string.Empty;
                    uriBuilder.Query = query.ToString();
                    return ResultBox.FromValue(uriBuilder.Uri.ToString());
                })
            .Conveyor(
                uri => ResultBox.WrapTry(
                    () =>
                        httpClient.GetAsync(
                            uri,
                            cancellationToken)))
            .Do(async response => Debug.Print(await response.Content.ReadAsStringAsync()))
            .Conveyor(
                response =>
                    response.GetResultFromJsonAsync<ListQueryResult<OamTermTenantsListQuery.Record>>(
                        cancellationToken));


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

    public Task<ResultBox<UnitValue>> AddNextMonth(
        CheckOrStartTenantTermAndAddAllUserNextMonth input,
        CancellationToken cancellationToken = default) =>
        ResultBox
            .Start
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/admin/startmonth",
                        input,
                        cancellationToken)))
            .Conveyor(
                response =>
                    ResultBox.FromValue(response.GetResultFromJsonAsync<CommandExecutorResponse>(cancellationToken)))
            .Remap(_ => UnitValue.Unit);

    public Task<ResultBox<UnitValue>> AddAllUserToTermTenant(
        AddAllUserToTermTenant input,
        CancellationToken cancellationToken = default) =>
        ResultBox
            .Start
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "admin/addallusertoterm",
                        input,
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
