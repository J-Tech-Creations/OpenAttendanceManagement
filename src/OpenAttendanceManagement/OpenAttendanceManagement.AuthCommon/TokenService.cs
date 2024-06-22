using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OpenAttendanceManagement.Common.Exceptions;
using ResultBoxes;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace OpenAttendanceManagement.AuthCommon;

public class TokenService(ProtectedSessionStorage protectedSessionStorage, HttpClient httpClient)
{
    private const string TokenKey = "authToken";
    public OptionalValue<string> SavedToken { get; private set; } = OptionalValue<string>.Empty;
    public bool IsSiteAdmin => Roles.Contains("SiteAdmin");
    public List<string> Roles { get; private set; } = new();
    public bool HasToken => SavedToken.HasValue;

    public Task<ResultBox<UnitValue>> LogOutAsync()
        => ResultBox.WrapTry(
            async () =>
            {
                await protectedSessionStorage.DeleteAsync(TokenKey);
                SavedToken = OptionalValue<string>.Empty;
                Roles = new List<string>();
                return UnitValue.Unit;
            });

    public Task<ResultBox<UnitValue>> SaveTokenAsync(string token)
        => ResultBox.WrapTry(
                async () =>
                {
                    await protectedSessionStorage.SetAsync(TokenKey, token);
                    return UnitValue.Unit;
                })
            .Do(_ => SavedToken = OptionalValue<string>.FromValue(token));
    public Task<ResultBox<string>> GetTokenAsync() =>
        SavedToken.HasValue
            ? ResultBox.FromValue(SavedToken.GetValue()).ToTask()
            : ResultBox.WrapTry(
                    async () => await protectedSessionStorage.GetAsync<string>(TokenKey))
                .Conveyor(
                    result => result.Success ?　ResultBox.CheckNull(
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
                        .ScanResult(result => Console.WriteLine(result))
                        .Conveyor(
                            result => result.HasValue ? ResultBox.FromValue(result.GetValue())
                                : ResultBox<List<string>>.Error(
                                    new TokenGetException("ロールが見つかりませんでした。")))
                        .Do(
                            list =>
                            {
                                Roles = list;
                            }));

    public ResultBox<UnitValue> RemoveTokenAsync()
        => ResultBox.WrapTry(async () => await protectedSessionStorage.DeleteAsync(TokenKey))
            .Remap(_ => UnitValue.Unit);
}
