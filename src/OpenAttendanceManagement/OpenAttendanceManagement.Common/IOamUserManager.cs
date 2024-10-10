using ResultBoxes;
namespace OpenAttendanceManagement.Common;

public interface IOamUserManager
{
    Task<ResultBox<string>> GetExecutingUserEmail();

    Task<ResultBox<OptionalValue<string>>> GetUserIdFromEmail(string email);
}
