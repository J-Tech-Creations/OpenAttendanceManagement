using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using Sekiban.Core.Events;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record OamTenantNameChanged(TenantName TenantName)
    : IEventPayload<OamTenant, OamTenantNameChanged>
{
    public static OamTenant OnEvent(OamTenant aggregatePayload, Event<OamTenantNameChanged> ev) =>
        aggregatePayload with
        {
            TenantName = ev.Payload.TenantName
        };
}