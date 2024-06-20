using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record CreateOamTenant(TenantCode TenantCode, TenantName TenantName)
    : ITenantCommandWithHandler<OamTenant, CreateOamTenant>
{
    public string TenantId => TenantCode.ToString();
    public Guid GetAggregateId() => Guid.NewGuid();
    public static ResultBox<UnitValue> HandleCommand(
        CreateOamTenant command,
        ICommandContext<OamTenant> context) =>
        context.AppendEvent(new OamTenantCreated(command.TenantCode, command.TenantName));
}
