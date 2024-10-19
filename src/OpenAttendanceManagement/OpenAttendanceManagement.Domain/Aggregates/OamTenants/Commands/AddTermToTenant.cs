using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record AddTermToTenant(
    TenantCode TenantCode,
    OamTenantId TenantId,
    OamTerm Term,
    OamTermTenantId TermTenantId,
    int ReferenceVersion)
    : ITenantCommandWithHandlerWithVersionValidationForExistingAggregate<OamTenant, AddTermToTenant>
{

    public static Guid SpecifyAggregateId(AddTermToTenant command) => command.TenantId.Value;
    public string GetTenantId() => TenantCode.Value;
    public static ResultBox<EventOrNone<OamTenant>> HandleCommand(
        AddTermToTenant command,
        ICommandContext<OamTenant> context) =>
        context.AppendEvent(new TermAddedToTenant(command.Term, command.TermTenantId));
}
