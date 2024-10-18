using OpenAttendanceManagement.Common.UseCases;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.Commands;
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
public record AddAllUserToTermTenant(
    TenantCode TenantCode,
    OamTenantId TenantId,
    OamTermTenantId TermTenantId) : ISekibanUsecaseAsync<AddAllUserToTermTenant, bool>
{
    public static Task<ResultBox<bool>> ExecuteAsync(AddAllUserToTermTenant input, ISekibanUsecaseContext context) =>
        // get all users in tenant (Get Tenant and it has all users)
        context
            .GetAggregateState<OamTenant>(input.TenantId.Value, input.TenantCode.Value)
            .Combine(
                state => ResultBox.FromValue(
                    state
                        .Payload
                        .Users
                        .OfType<OamConfirmedTenantUserInformation>()
                        .ToList()))
            // get term tenant
            .Combine(
                (_, _) => context.GetAggregateState<OamTermTenant>(input.TermTenantId.Value, input.TenantCode.Value))
            // create term user for each user
            .Conveyor(
                (tenant, users, termtenant) =>
                    // reduce all users
                    ResultBox
                        .FromValue(users)
                        .ReduceEach(
                            UnitValue.Unit,
                            (user, _) =>
                                ResultBox
                                    .FromValue(user)
                                    // check if user already in term tenant
                                    .Remap(
                                        u => termtenant.Payload.TermUsers.Any(
                                            t => t.TenantUserId.Equals(u.TenantUserId)))
                                    // add term user to term tenant
                                    .Conveyor(
                                        exists => exists
                                            ? ResultBox.UnitValue.ToTask()
                                            : context
                                                .GetAggregateState<OamTenantUser>(
                                                    user.TenantUserId.Value,
                                                    input.TenantCode.Value)
                                                .Conveyor(
                                                    tenantUser => context.ExecuteCommand(
                                                        new CreateOamTermUser(
                                                            input.TenantCode,
                                                            input.TermTenantId,
                                                            user.TenantUserId,
                                                            tenantUser.Payload.UserName)))
                                                .Remap(_ => UnitValue.Unit)
                                    )
                        )
            )
            .Remap(_ => true);
}
