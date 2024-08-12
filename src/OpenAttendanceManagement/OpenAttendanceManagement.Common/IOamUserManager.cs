using Microsoft.AspNetCore.Identity;
using ResultBoxes;

namespace OpenAttendanceManagement.Common;

public interface IOamUserManager
{
    Task<ResultBox<IdentityUser>> GetExecutingUser();
    Task<ResultBox<string>> GetExecutingUserEmail();

    Task<ResultBox<OptionalValue<string>>> GetUserIdFromEmail(string email);
}