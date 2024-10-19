using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Commands;

public record AddOamTermUserToTermTenant(
    TenantCode TenantCode,
    OamTermTenantId TermTenantId,
    OamTeamUserWithUserId User)
    : ITenantCommandWithHandlerForExistingAggregate<OamTermTenant, AddOamTermUserToTermTenant>
{
    public static Guid SpecifyAggregateId(AddOamTermUserToTermTenant command) => command.TermTenantId.Value;
    public string GetTenantId() => TenantCode.Value;
    public static ResultBox<EventOrNone<OamTermTenant>> HandleCommand(
        AddOamTermUserToTermTenant command,
        ICommandContext<OamTermTenant> context) => EventOrNone.Event(new OamTermTenantUserAdded(command.User));
}
