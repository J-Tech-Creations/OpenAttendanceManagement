using System.Diagnostics;
using System.Web;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.Queries;
using OpenAttendanceManagement.Domain.Aggregates.Queries;
using ResultBoxes;
using Sekiban.Core.Query.QueryModel;

namespace OpenAttendanceManagement.Web.Apis;

public class UserApiClient(HttpClient httpClient, TokenService tokenService, TenantInformation tenantInformation)
{
    public Task<ResultBox<ListQueryResult<OamTenantUsersQuery.Record>>> GetTenantUsers(
        CancellationToken cancellationToken = default) =>
        tokenService.SetTokenToHeader(httpClient)
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


    public Task<ResultBox<MyUserInformationQuery.Result>> GetMyTenantUser(
        CancellationToken cancellationToken = default) =>
        tokenService.SetTokenToHeader(httpClient)
            .Conveyor(() => ResultBox.CheckNull(httpClient.BaseAddress))
            .Conveyor(baseAddress =>
            {
                var uriBuilder = new UriBuilder(baseAddress.Scheme, baseAddress.Host, baseAddress.Port,
                    "/api/query/myuserinformationquery");
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
                    response.GetResultFromJsonAsync<MyUserInformationQuery.Result>(cancellationToken));

    public Task<ResultBox<UnitValue>> CreateUser(
        CreateUserModel command,
        CancellationToken cancellationToken = default) =>
        ResultBox.Start
            .Conveyor(
                async _ => ResultBox.CheckNull(
                    await httpClient.PostAsJsonAsync(
                        "/register",
                        command,
                        cancellationToken)))
            .Verify(response => response.IsSuccessStatusCode
                ? ExceptionOrNone.None
                : ExceptionOrNone.FromException(new ApplicationException("Failed to create user")))
            .Remap(_ => UnitValue.Unit);

    public record CreateUserModel(string Email, string Password);
}