using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObject;
using Sekiban.Core.Aggregate;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants;

public record OamTenant(TenantCode TenantCode, TenantName TenantName) : IAggregatePayload<OamTenant>
{
    public static OamTenant CreateInitialPayload(OamTenant? _) =>
        new(TenantCode.Default, TenantName.Default);
}
