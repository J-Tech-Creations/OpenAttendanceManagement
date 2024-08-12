using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Events;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record OamTenantUserAddedToTenant(
    AuthIdentityEmail Email,
    OptionalValue<AuthIdentityId> AuthIdentityId,
    OamTenantUserId UserId,
    OamTenantId TenantId,
    OamUserName UserName,
    OamDisplayName DisplayName)
    : IEventPayload<OamTenant, OamTenantUserAddedToTenant>
{
    public static OamTenant OnEvent(OamTenant aggregatePayload, Event<OamTenantUserAddedToTenant> ev) =>
        aggregatePayload with
        {
            Users = aggregatePayload.Users.All(u => !u.AuthIdentityEmail.NormalizedEquals(ev.Payload.Email))
                ? aggregatePayload.Users.Add(new OamUnconfirmedTenantUserInformation(ev.Payload.UserId,
                    ev.Payload.AuthIdentityId, ev.Payload.Email))
                : aggregatePayload.Users
        };
}