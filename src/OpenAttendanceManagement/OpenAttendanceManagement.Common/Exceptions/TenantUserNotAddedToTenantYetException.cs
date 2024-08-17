namespace OpenAttendanceManagement.Common.Exceptions;

public class TenantUserNotAddedToTenantYetException(string message) : Exception(message)
{
}