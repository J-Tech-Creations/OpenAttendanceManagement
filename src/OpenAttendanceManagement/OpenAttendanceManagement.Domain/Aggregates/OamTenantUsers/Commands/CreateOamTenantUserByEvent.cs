using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Events;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.Commands;

public record CreateOamTenantUserByEvent(Event<OamTenantUserAddedToTenant> TenantEvent)
    : ITenantCommandWithHandlerAsync<OamTenantUser, CreateOamTenantUserByEvent>
{
    public Guid GetAggregateId() => TenantEvent.Payload.UserId.Value;

    public static Task<ResultBox<UnitValue>> HandleCommandAsync(CreateOamTenantUserByEvent command,
        ICommandContext<OamTenantUser> context) => context.AppendEvent(new OamTenantUserCreatedByEvent(
        command.TenantEvent.Payload.TenantId, command.TenantEvent.Payload.AuthIdentityId,
        command.TenantEvent.Payload.Email, command.TenantEvent.Payload.UserName,
        command.TenantEvent.Payload.DisplayName)).ToTask();

    public string TenantId => TenantEvent.RootPartitionKey;
}

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