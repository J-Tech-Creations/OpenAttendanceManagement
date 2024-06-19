using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record OamTenantCreated(TenantCode TenantCode, TenantName TenantName)
{
    
}
