using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using Sekiban.Core.Events;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.Commands;

public record OamTermUserCreated(
    OamTermTenantId TermTenantId,
    OamTenantUserId TenantUserId,
    OamUserName UserName) : IEventPayload<OamTermUser, OamTermUserCreated>
{

    public static OamTermUser OnEvent(OamTermUser aggregatePayload, Event<OamTermUserCreated> ev) => aggregatePayload
        with
        {
            TermTenantId = ev.Payload.TermTenantId,
            TenantUserId = ev.Payload.TenantUserId,
            UserName = ev.Payload.UserName
        };
}
