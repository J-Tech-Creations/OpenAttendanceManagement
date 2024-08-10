using ResultBoxes;

namespace OpenAttendanceManagement.Common;

public interface IOatAuthentication
{
    public Task<ResultBox<OatLoginUser>> GetOatLoginUser();
}