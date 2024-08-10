using Microsoft.AspNetCore.Identity;
using ResultBoxes;

namespace OpenAttendanceManagement.Common;

public interface IOamUserManager
{
    Task<ResultBox<IdentityUser>> GetExecutingUser();
    Task<ResultBox<string>> GetExecutingUserEmail();

    Task<ResultBox<string>> GetUserIdFromEmail(string email);
}