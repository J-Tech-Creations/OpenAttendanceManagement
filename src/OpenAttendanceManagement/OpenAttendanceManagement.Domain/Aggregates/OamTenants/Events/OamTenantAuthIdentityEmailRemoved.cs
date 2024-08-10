using System.Collections.Immutable;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using Sekiban.Core.Events;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record OamTenantAuthIdentityEmailRemoved(AuthIdentityEmail AuthIdentityEmail)
    : IEventPayload<OamTenant, OamTenantAuthIdentityEmailRemoved>
{
    public static OamTenant OnEvent(
        OamTenant aggregatePayload,
        Event<OamTenantAuthIdentityEmailRemoved> ev) =>
        aggregatePayload with
        {
            Admins = aggregatePayload.Admins
                .Where(email => email.Value != ev.Payload.AuthIdentityEmail.Value)
                .ToImmutableList()
        };
}