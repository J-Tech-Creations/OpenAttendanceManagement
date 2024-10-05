using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OpenAttendanceManagement.Common.Exceptions;
using ResultBoxes;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace OpenAttendanceManagement.AuthCommon;

public class TokenService(
    HttpClient httpClient,
    ProtectedSessionStorage protectedSessionStorage)
{
    private const string TokenKey = "authToken";
    private const string EmailKey = "emailKey";
    public bool IsSiteAdmin => Roles.Contains("SiteAdmin");
    public List<string> Roles { get; private set; } = new();

    public Task<ResultBox<string>> GetTokenAsync() =>
        ResultBox
            .WrapTry(
                async () => await protectedSessionStorage.GetAsync<string>(TokenKey))
            .Conveyor(
                result => result.Success
                    ?　ResultBox.CheckNull(
                        result.Value,
                        new TokenGetException("トークンが見つかりませんでした。(Null)"))
                    : ResultBox<string>.Error(new TokenGetException("トークンが見つかりませんでした。")));

    public Task<ResultBox<UnitValue>> SetTokenAsync(string token) =>
        ResultBox.WrapTry(
            async () =>
            {
                await protectedSessionStorage.SetAsync(TokenKey, token);
                return UnitValue.Unit;
            });

    public Task<ResultBox<UnitValue>> SetEmailAsync(string email) =>
        ResultBox.WrapTry(
            async () =>
            {
                await protectedSessionStorage.SetAsync(EmailKey, email);
                return UnitValue.Unit;
            });

    public Task<ResultBox<string>> GetEmailAsync() =>
        ResultBox
            .WrapTry(
                async () => await protectedSessionStorage.GetAsync<string>(EmailKey))
            .Conveyor(
                result => result.Success
                    ?　ResultBox.CheckNull(
                        result.Value,
                        new TokenGetException("メールアドレスが見つかりませんでした。(Null)"))
                    : ResultBox<string>.Error(new TokenGetException("メールアドレスが見つかりませんでした。")));

    public Task<ResultBox<UnitValue>> LogOutAsync() =>
        ResultBox.WrapTry(
            async () =>
            {
                await protectedSessionStorage.DeleteAsync(TokenKey);
                await protectedSessionStorage.DeleteAsync(EmailKey);
                return UnitValue.Unit;
            });

    public Task<ResultBox<UnitValue>> SetTokenToHeader(HttpClient httpClient)
        => GetTokenAsync()
            .Do(
                token =>
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token))
            .Conveyor(() => ResultBox.UnitValue);

    public Task<ResultBox<UnitValue>> SaveTokenAndEmailAsync(string token, string email) =>
        SetTokenAsync(token)
            .Conveyor(() => SetEmailAsync(email));


    public Task<ResultBox<UnitValue>> UpdateRoleAsync()
        => SetTokenToHeader(httpClient)
            .Do(
                _ => ResultBox
                    .WrapTry(
                        async () => new OptionalValue<List<string>>(
                            await httpClient.GetFromJsonAsync<List<string>>(
                                "/user/roles")))
                    .ScanResult(result => result.Log("after get roles[TokenService]"))
                    .Conveyor(
                        result => result.HasValue
                            ? ResultBox.FromValue(result.GetValue())
                            : ResultBox<List<string>>.Error(
                                new TokenGetException("ロールが見つかりませんでした。")))
                    .Do(
                        list => { Roles = list; }))
            .Conveyor(() => ResultBox.UnitValue);
}
