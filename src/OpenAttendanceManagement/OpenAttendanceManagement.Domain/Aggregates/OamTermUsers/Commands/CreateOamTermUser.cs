using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.Commands;

public record CreateOamTermUser(
    TenantCode TenantCode,
    OamTermTenantId TermTenantId,
    OamTenantUserId TenantUserId,
    OamUserName UserName) : ITenantCommandWithHandler<OamTermUser, CreateOamTermUser>
{

    public static Guid SpecifyAggregateId(CreateOamTermUser command) => Guid.NewGuid();
    public static ResultBox<EventOrNone<OamTermUser>> HandleCommand(
        CreateOamTermUser command,
        ICommandContext<OamTermUser> context) =>
        context.AppendEvent(new OamTermUserCreated(command.TermTenantId, command.TenantUserId, command.UserName));
    public string GetTenantId() => TenantCode.Value;
}
