using OpenAttendanceManagement.Common.Exceptions;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record CreateOamTenant(TenantCode TenantCode, TenantName TenantName)
    : ICommandWithHandlerAsync<OamTenant, CreateOamTenant>
{
    public string TenantId => TenantCode.Value;
    public Guid GetAggregateId() => Guid.NewGuid();
    public static Task<ResultBox<UnitValue>> HandleCommandAsync(
        CreateOamTenant command,
        ICommandContext<OamTenant> context) => context.GetRequiredService<IQueryExecutor>()
        .Conveyor(
            executor => executor.ExecuteAsync(new TenantCodeExistsQuery(command.TenantCode)))
        .Verify(
            result => result ? new TenantCodeDuplicateException(command.TenantCode.Value)
                : ExceptionOrNone.None)
        .Conveyor(
            _ => context.AppendEvent(
                new OamTenantCreated(command.TenantCode, command.TenantName)));
}
