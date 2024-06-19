namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

public record OamTenantUserId(string Value)
{
    public static OamTenantUserId Default => new(string.Empty);
}
