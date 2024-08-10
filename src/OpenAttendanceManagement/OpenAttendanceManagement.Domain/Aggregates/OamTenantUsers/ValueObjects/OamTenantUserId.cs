namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

public record OamTenantUserId(Guid Value)
{
    public static OamTenantUserId Default => new(Guid.Empty);
}