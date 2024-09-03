namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamTenantId(Guid Value)
{
    public static OamTenantId Default => new(Guid.Empty);
}