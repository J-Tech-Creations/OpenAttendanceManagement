using OpenAttendanceManagement.Common;
using ResultBoxes;
namespace OpenAttendanceManagement.AuthCommon;

public class OamUserManagerKeycloak(
    TokenServiceKeycloak tokenServiceKeycloak) : IOamUserManager
{
    public Task<ResultBox<string>> GetExecutingUserEmail() =>
        tokenServiceKeycloak.GetEmailAsync();

    public Task<ResultBox<OptionalValue<string>>> GetUserIdFromEmail(string email) =>
        tokenServiceKeycloak
            .GetIdAsync()
            .Match(success => ResultBox.FromValue(OptionalValue.FromValue(success)), _ => OptionalValue<string>.Null);
}
