using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record DeleteOamTenant(OamTenantId OamTenantId)
    : ICommandWithHandler<OamTenant, DeleteOamTenant>
{
    public Guid GetAggregateId() => OamTenantId.Value;
    public static ResultBox<UnitValue> HandleCommand(
        DeleteOamTenant command,
        ICommandContext<OamTenant> context) => context.AppendEvent(new OamTenantDeleted());
}