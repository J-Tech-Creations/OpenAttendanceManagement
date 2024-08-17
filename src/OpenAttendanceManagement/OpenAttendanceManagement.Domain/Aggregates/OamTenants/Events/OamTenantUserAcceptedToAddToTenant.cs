using System.Collections.Immutable;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using Sekiban.Core.Events;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record OamTenantUserAcceptedToAddToTenant(
    AuthIdentityEmail Email)
    : IEventPayload<OamTenant, OamTenantUserAcceptedToAddToTenant>
{
    public static OamTenant OnEvent(OamTenant aggregatePayload, Event<OamTenantUserAcceptedToAddToTenant> ev) =>
        aggregatePayload with
        {
            Users = aggregatePayload.Users.Select(user =>
                user.AuthIdentityEmail.NormalizedEquals(ev.Payload.Email)
                && user is OamUnconfirmedTenantUserInformation { AuthIdentityId.HasValue: true } unconfirmedUser
                    ? new OamConfirmedTenantUserInformation(unconfirmedUser.TenantUserId,
                        unconfirmedUser.AuthIdentityId.GetValue(), user.AuthIdentityEmail)
                    : user).ToImmutableList()
        };
}