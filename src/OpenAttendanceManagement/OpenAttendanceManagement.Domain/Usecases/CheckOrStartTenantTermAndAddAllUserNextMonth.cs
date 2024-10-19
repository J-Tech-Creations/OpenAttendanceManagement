using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Queries;
using ResultBoxes;
using Sekiban.Core.Usecase;
namespace OpenAttendanceManagement.Domain.Usecases;

public record CheckOrStartTenantTermAndAddAllUserNextMonth(
    TenantCode TenantCode,
    OamTenantId TenantId) : ISekibanUsecaseAsync<CheckOrStartTenantTermAndAddAllUserNextMonth, UnitValue>
{
    public static Task<ResultBox<UnitValue>> ExecuteAsync(
        CheckOrStartTenantTermAndAddAllUserNextMonth input,
        ISekibanUsecaseContext context) =>
        context
            .ExecuteQuery(new OamTermTenantsListQuery(input.TenantCode, input.TenantId))
            .Remap(
                list => list.TotalCount == 0
                    ? DateTime.Today
                    : list.Items.Last().StartDate.AddMonths(1).ToDateTime(TimeOnly.MinValue))
            .Remap(
                date => new CheckOrStartTenantTermAndAddAllUser(
                    input.TenantCode,
                    input.TenantId,
                    date.Year,
                    date.Month))
            .Conveyor(context.ExecuteUsecase)
            .Remap(_ => UnitValue.Unit);
}
