using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using Sekiban.Core.Events;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Events;

public record OamTermTenantCreated(OamTerm Term) : IEventPayload<OamTermTenant, OamTermTenantCreated>
{
    public static OamTermTenant OnEvent(OamTermTenant aggregatePayload, Event<OamTermTenantCreated> ev) =>
        aggregatePayload with
        {
            Term = ev.Payload.Term
        };
}
