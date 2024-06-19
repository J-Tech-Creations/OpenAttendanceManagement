namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

public record OamUserName(string FirstName, string LastName)
{
    public static OamUserName Default => new(string.Empty, string.Empty);
}
