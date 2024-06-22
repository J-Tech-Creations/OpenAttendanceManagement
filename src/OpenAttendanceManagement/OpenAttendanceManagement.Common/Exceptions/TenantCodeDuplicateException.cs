namespace OpenAttendanceManagement.Common.Exceptions;

public class TenantCodeDuplicateException(string message) : ApplicationException(message)
{
}
