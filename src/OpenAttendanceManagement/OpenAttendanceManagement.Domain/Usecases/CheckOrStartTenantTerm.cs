using OpenAttendanceManagement.Domain.Aggregates.OamTenants;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Usecase;
using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Usecases;

public record CheckOrStartTenantTerm(
    TenantCode TenantCode,
    OamTenantId TenantId,
    [property: Range(2000, 2100)]
    int Year,
    [property: Range(1, 12)]
    int Month)
    : ISekibanUsecaseAsync<CheckOrStartTenantTerm, OamTermTenantId>
{
    public static Task<ResultBox<OamTermTenantId>> ExecuteAsync(
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
            .Combine(
                (state, term, _, aggregateId) => context.ExecuteCommand(
                    new AddTermToTenant(
                        input.TenantCode,
                        input.TenantId,
                        term,
                        new OamTermTenantId(aggregateId),
                        state.Version)))
            .Remap(
                (
                    _,
                    _,
                    _,
                    aggregateId,
                    _) => new OamTermTenantId(aggregateId));
}
