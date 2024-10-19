using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Usecase;
using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Usecases;

public record CheckOrStartTenantTermAndAddAllUser(
    TenantCode TenantCode,
    OamTenantId TenantId,
    [property: Range(2000, 2100)]
    int Year,
    [property: Range(1, 12)]
    int Month) : ISekibanUsecaseAsync<CheckOrStartTenantTermAndAddAllUser, UnitValue>
{

    public static Task<ResultBox<UnitValue>> ExecuteAsync(
        CheckOrStartTenantTermAndAddAllUser input,
        ISekibanUsecaseContext context) =>
        context
            .ExecuteUsecase(new CheckOrStartTenantTerm(input.TenantCode, input.TenantId, input.Year, input.Month))
            .Combine(
                termTenantId => context.ExecuteUsecase(
                    new AddAllUserToTermTenant(
                        input.TenantCode,
                        input.TenantId,
                        termTenantId)))
            .Remap(_ => UnitValue.Unit);
}
