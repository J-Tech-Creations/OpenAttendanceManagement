using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record ChangeOamTenantName(
    OamTenantId OamTenantId,
    TenantCode TenantCode,
    TenantName TenantName)
    : ICommandWithHandlerAsync<OamTenant, ChangeOamTenantName>
{
    public string TenantId => TenantCode.Value;
    public Guid GetAggregateId() => OamTenantId.Value;
    public static Task<ResultBox<UnitValue>> HandleCommandAsync(
        ChangeOamTenantName command,
        ICommandContext<OamTenant> context) =>
        context.AppendEvent(new OamTenantNameChanged(command.TenantName)).ToTask();
}
