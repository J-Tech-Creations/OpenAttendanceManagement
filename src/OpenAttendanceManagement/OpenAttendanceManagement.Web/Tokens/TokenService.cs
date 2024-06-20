using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OpenAttendanceManagement.Web.Exceptions;
using ResultBoxes;
namespace OpenAttendanceManagement.Web.Tokens;

public class TokenService(ProtectedSessionStorage protectedSessionStorage)
{
    private const string TokenKey = "authToken";
    public OptionalValue<string> SavedToken { get; private set; } = OptionalValue<string>.Empty;
    public bool HasToken => SavedToken.HasValue;

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
                .Do(token => SavedToken = token);

    public ResultBox<UnitValue> RemoveTokenAsync()
        => ResultBox.WrapTry(async () => await protectedSessionStorage.DeleteAsync(TokenKey))
            .Remap(_ => UnitValue.Unit);
}
