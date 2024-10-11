using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Events;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.Commands;

public record CreateOamTermUser(
    TenantCode TenantCode,
    OamTermTenantId TermTenantId,
    OamTenantUserId TenantUserId,
    OamUserName UserName) : ITenantCommandWithHandler<OamTermUser, CreateOamTermUser>
{

    public static Guid SpecifyAggregateId(CreateOamTermUser command) => Guid.NewGuid();
    public static ResultBox<UnitValue> HandleCommand(CreateOamTermUser command, ICommandContext<OamTermUser> context) =>
        context.AppendEvent(new OamTermUserCreated(command.TermTenantId, command.TenantUserId, command.UserName));
    public string GetTenantId() => TenantCode.Value;
}
public record OamTermUserCreated(
    OamTermTenantId TermTenantId,
    OamTenantUserId TenantUserId,
    OamUserName UserName) : IEventPayload<OamTermUser, OamTermUserCreated>
{

    public static OamTermUser OnEvent(OamTermUser aggregatePayload, Event<OamTermUserCreated> ev) => aggregatePayload
        with
        {
            TermTenantId = ev.Payload.TermTenantId,
            TenantUserId = ev.Payload.TenantUserId,
            UserName = ev.Payload.UserName
        };
}
