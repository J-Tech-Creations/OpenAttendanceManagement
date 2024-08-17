using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OpenAttendanceManagement.Common.Exceptions;
using ResultBoxes;

namespace OpenAttendanceManagement.AuthCommon;

public class TokenService(ProtectedSessionStorage protectedSessionStorage, HttpClient httpClient)
{
    private const string TokenKey = "authToken";
    private const string EmailKey = "emailKey";
    public OptionalValue<string> SavedToken { get; private set; } = OptionalValue<string>.Empty;
    public OptionalValue<string> SavedEmail { get; private set; } = OptionalValue<string>.Empty;
    public bool IsSiteAdmin => Roles.Contains("SiteAdmin");
    public List<string> Roles { get; private set; } = new();
    public bool HasToken => SavedToken.HasValue;

    public Task<ResultBox<UnitValue>> LogOutAsync()
        => ResultBox.WrapTry(
            async () =>
            {
                await protectedSessionStorage.DeleteAsync(TokenKey);
                await protectedSessionStorage.DeleteAsync(EmailKey);
                SavedToken = OptionalValue<string>.Empty;
                Roles = new List<string>();
                return UnitValue.Unit;
            });

    public Task<ResultBox<UnitValue>> SaveTokenAsync(string token, string email)
        => ResultBox.WrapTry(
                async () =>
                {
                    await protectedSessionStorage.SetAsync(TokenKey, token);
                    return UnitValue.Unit;
                })
            .Do(_ => SavedToken = OptionalValue.FromValue(token))
            .Conveyor(() => ResultBox.WrapTry(
                async () =>
                {
                    await protectedSessionStorage.SetAsync(EmailKey, email);
                    return UnitValue.Unit;
                }))
            .Do(_ => SavedEmail = OptionalValue.FromValue(email));

    public Task<ResultBox<string>> GetTokenAndRoleAsync() =>
        ResultBox.WrapTry(
                async () => await protectedSessionStorage.GetAsync<string>(TokenKey))
            .Conveyor(
                result => result.Success
                    ?　ResultBox.CheckNull(
                        result.Value,
                        new TokenGetException("トークンが見つかりませんでした。(Null)"))
                    : ResultBox<string>.Error(new TokenGetException("トークンが見つかりませんでした。")))
            .Do(token => SavedToken = token)
            .Do(
                token => httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token))
            .Do(
                _ => ResultBox.WrapTry(
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
            .Conveyor(() => ResultBox.WrapTry(
                async () => await protectedSessionStorage.GetAsync<string>(EmailKey)))
            .Conveyor(emailStorage => ResultBox.CheckNull(emailStorage.Value))
            .Do(email => SavedEmail = email)
            .Conveyor(() => SavedToken.HasValue
                ? ResultBox.FromValue(SavedToken.GetValue())
                : ResultBox<string>.Error(new TokenGetException("トークンが見つかりませんでした。")));
}