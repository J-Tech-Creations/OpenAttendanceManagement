using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using Sekiban.Core.Events;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record OamTenantAuthIdentityEmailAdded(AuthIdentityEmail AuthIdentityEmail)
    : IEventPayload<OamTenant, OamTenantAuthIdentityEmailAdded>
{
    public static OamTenant OnEvent(
        OamTenant aggregatePayload,
        Event<OamTenantAuthIdentityEmailAdded> ev) =>
        aggregatePayload with
        {
            Admins = aggregatePayload.Admins.Add(ev.Payload.AuthIdentityEmail)
        };
}