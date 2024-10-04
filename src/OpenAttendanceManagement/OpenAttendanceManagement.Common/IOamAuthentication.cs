using ResultBoxes;
namespace OpenAttendanceManagement.Common;

public interface IOamAuthentication
{
    public Task<ResultBox<OatLoginUser>> GetOatLoginUser();
}
