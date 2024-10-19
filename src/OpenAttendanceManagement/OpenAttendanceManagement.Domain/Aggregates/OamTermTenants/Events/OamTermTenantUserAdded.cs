using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.ValueObjects;
using Sekiban.Core.Events;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Events;

public record OamTermTenantUserAdded(OamTeamUserWithUserId User) : IEventPayload<OamTermTenant, OamTermTenantUserAdded>
{
    public static OamTermTenant OnEvent(OamTermTenant aggregatePayload, Event<OamTermTenantUserAdded> ev) =>
        aggregatePayload with { TermUsers = aggregatePayload.TermUsers.Add(ev.Payload.User) };
}
