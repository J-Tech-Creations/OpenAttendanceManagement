using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using Sekiban.Core.Events;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record TermAddedToTenant(
    OamTerm Term,
    OamTermTenantId TermTenantId) : IEventPayload<OamTenant, TermAddedToTenant>
{
    public static OamTenant OnEvent(OamTenant aggregatePayload, Event<TermAddedToTenant> ev) => aggregatePayload with
    {
        Terms = aggregatePayload.Terms.Add(ev.Payload.Term, ev.Payload.TermTenantId)
    };
}
