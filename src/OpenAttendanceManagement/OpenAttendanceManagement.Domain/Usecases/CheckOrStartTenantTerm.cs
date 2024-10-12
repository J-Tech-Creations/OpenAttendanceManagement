using OpenAttendanceManagement.Common.UseCases;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using ResultBoxes;
using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Usecases;

public record CheckOrStartTenantTerm(
    TenantCode TenantCode,
    OamTenantId TenantId,
    [property: Range(2000, 2100)]
    int Year,
    [property: Range(1, 12)]
    int Month)
    : ISekibanUsecaseAsync<CheckOrStartTenantTerm, UnitValue>
{
    public static Task<ResultBox<UnitValue>> ExecuteAsync(
        CheckOrStartTenantTerm input,
        ISekibanUsecaseContext context) =>
        context
            .GetAggregateState<OamTenant>(input.TenantId.Value, input.TenantCode.Value)
            .Combine(() => ResultBox.FromValue(OamTerm.MonthTerm(input.Year, input.Month)))
            .Verify(
                (state, term) => !state.Payload.Terms.ContainsKey(term)
                    ? ExceptionOrNone.None
                    : new ApplicationException("Term already exists"))
            .Combine((_, term) => context.ExecuteCommand(new CreateOamTermTenant(input.TenantCode, term)))
            .Combine((_, _, executed) => ResultBox.CheckNull(executed.AggregateId))
            .Conveyor(
                (state, term, _, aggregateId) => context.ExecuteCommand(
                    new AddTermToTenant(
                        input.TenantCode,
                        input.TenantId,
                        term,
                        new OamTermTenantId(aggregateId),
                        state.Version)))
            .Conveyor(_ => ResultBox.Start);
}
