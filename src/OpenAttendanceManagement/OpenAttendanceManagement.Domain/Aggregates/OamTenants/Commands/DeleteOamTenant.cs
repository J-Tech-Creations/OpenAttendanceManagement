using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record DeleteOamTenant(OamTenantId OamTenantId, TenantCode TenantCode)
    : ITenantCommandWithHandler<OamTenant, DeleteOamTenant>
{
    public static ResultBox<EventOrNone<OamTenant>> HandleCommand(
        DeleteOamTenant command,
        ICommandContext<OamTenant> context) => context.AppendEvent(new OamTenantDeleted());

    public static Guid SpecifyAggregateId(DeleteOamTenant command) => command.OamTenantId.Value;
    public string GetTenantId() => TenantCode.Value;
}
