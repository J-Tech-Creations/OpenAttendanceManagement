using Sekiban.Core.Events;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record OamTenantDeleted : IEventPayload<OamTenant, OamTenantDeleted>
{
    public static OamTenant OnEvent(OamTenant aggregatePayload, Event<OamTenantDeleted> ev) =>
        aggregatePayload with
        {
            IsDeleted = true
        };
}