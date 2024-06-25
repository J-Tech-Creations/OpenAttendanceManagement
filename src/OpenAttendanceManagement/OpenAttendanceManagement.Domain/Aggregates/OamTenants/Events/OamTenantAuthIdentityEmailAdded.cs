using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using Sekiban.Core.Events;
using System.Collections.Immutable;
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
