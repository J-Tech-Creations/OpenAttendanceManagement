using OpenAttendanceManagement.Common.Exceptions;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record CreateOamTenant(TenantCode TenantCode, TenantName TenantName)
    : ITenantCommandWithHandlerAsync<OamTenant, CreateOamTenant>
{
    public static Task<ResultBox<UnitValue>> HandleCommandAsync(
        CreateOamTenant command,
        ICommandContext<OamTenant> context) =>
        context
            .ExecuteQueryAsync(new TenantCodeExistsQuery(command.TenantCode))
            .Verify(
                result => result
                    ? new TenantCodeDuplicateException(command.TenantCode.Value)
                    : ExceptionOrNone.None)
            .Conveyor(
                _ => context.AppendEvent(
                    new OamTenantCreated(command.TenantCode, command.TenantName)));
    public static Guid SpecifyAggregateId(CreateOamTenant command) => Guid.NewGuid();
    public string GetTenantId() => TenantCode.Value;
}
