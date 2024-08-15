using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
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