using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Events;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.Commands;

/// <summary>
/// </summary>
/// <param name="TenantId"></param>
/// <param name="AuthIdentityId">When OptionalValue.Empty, user have not created yet.</param>
/// <param name="Email"></param>
/// <param name="UserName"></param>
/// <param name="DisplayName"></param>
public record OamTenantUserCreatedByEvent(
    OamTenantId TenantId,
    OptionalValue<AuthIdentityId> AuthIdentityId,
    AuthIdentityEmail Email,
    OamUserName UserName,
    OamDisplayName DisplayName) : IEventPayload<OamTenantUser, OamTenantUserCreatedByEvent>
{
    public static OamTenantUser OnEvent(OamTenantUser aggregatePayload, Event<OamTenantUserCreatedByEvent> ev) =>
        new(ev.Payload.TenantId, ev.Payload.AuthIdentityId, ev.Payload.Email, ev.Payload.UserName,
            ev.Payload.DisplayName);
}