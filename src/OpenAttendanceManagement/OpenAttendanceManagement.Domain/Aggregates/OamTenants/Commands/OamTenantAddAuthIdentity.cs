using OpenAttendanceManagement.Common.Exceptions;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record OamTenantAddAuthIdentity(
    OamTenantId OamTenantId,
    AuthIdentityEmail Email,
    TenantCode TenantCode)
    : ITenantCommandWithHandler<OamTenant, OamTenantAddAuthIdentity>
{
    public static ResultBox<EventOrNone<OamTenant>> HandleCommand(
        OamTenantAddAuthIdentity command,
        ICommandContext<OamTenant> context) =>
        ResultBox
            .Start
            .Verify(
                _ => context.GetState().Payload.Admins.Any(x => x.Value == command.Email.Value)
                    ? new TenantAdminAlreadyExistsException(command.Email.Value + "はすでに存在しています。")
                    : ExceptionOrNone.None)
            .Conveyor(_ => context.AppendEvent(new OamTenantAuthIdentityEmailAdded(command.Email)));

    public static Guid SpecifyAggregateId(OamTenantAddAuthIdentity command) => command.OamTenantId.Value;
    public string GetTenantId() => TenantCode.Value;
}
