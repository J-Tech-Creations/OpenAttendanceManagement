using OpenAttendanceManagement.Common.Exceptions;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record OamTenantRemoveAuthIdentity(
    OamTenantId OamTenantId,
    AuthIdentityEmail Email,
    TenantCode TenantCode) : ITenantCommandWithHandler<OamTenant, OamTenantRemoveAuthIdentity>
{
    public static ResultBox<EventOrNone<OamTenant>> HandleCommand(
        OamTenantRemoveAuthIdentity command,
        ICommandContext<OamTenant> context) =>
        ResultBox
            .FromValue(context.GetState())
            .Verify(
                state => state.Payload.Admins.Any(x => x.Value == command.Email.Value)
                    ? ExceptionOrNone.None
                    : new TenantAdminNotFound("指定された管理者が見つかりません。"))
            .Conveyor(
                _ => context.AppendEvent(new OamTenantAuthIdentityEmailRemoved(command.Email)));

    public static Guid SpecifyAggregateId(OamTenantRemoveAuthIdentity command) => command.OamTenantId.Value;
    public string GetTenantId() => TenantCode.Value;
}
