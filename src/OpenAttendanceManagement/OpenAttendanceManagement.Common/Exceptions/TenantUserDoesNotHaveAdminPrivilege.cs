namespace OpenAttendanceManagement.Common.Exceptions;

public class TenantUserDoesNotHaveAdminPrivilege(string message) : Exception(message)
{
}