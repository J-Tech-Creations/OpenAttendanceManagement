using OpenAttendanceManagement.Domain.Aggregates.OamTenants;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.Queries;
using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Usecase;
namespace OpenAttendanceManagement.Domain.Usecases;

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
                                                .Combine(
                                                    tenantUser =>
                                                        context
                                                            .ExecuteQuery(
                                                                new OamTermUserIdSearch(
                                                                    input.TenantCode,
                                                                    input.TermTenantId,
                                                                    user.TenantUserId))
                                                            .Conveyor(
                                                                termUserId => termUserId.Match(
                                                                    some => some.ToResultBox().ToTask(),
                                                                    () => context
                                                                        .ExecuteCommand(
                                                                            new CreateOamTermUser(
                                                                                input.TenantCode,
                                                                                input.TermTenantId,
                                                                                user.TenantUserId,
                                                                                tenantUser.Payload.UserName))
                                                                        .Conveyor(
                                                                            response => response.AggregateId is not null
                                                                                ? new OamTermUserId(
                                                                                        response.AggregateId.Value)
                                                                                    .ToResultBox()
                                                                                : ResultBox<OamTermUserId>
                                                                                    .FromException(
                                                                                        new ApplicationException(
                                                                                            "CreateOamTermUser failed"))))))
                                                .Conveyor(
                                                    (tenantUser, termUserId) =>
                                                        context
                                                            .ExecuteCommand(
                                                                new AddOamTermUserToTermTenant(
                                                                    input.TenantCode,
                                                                    input.TermTenantId,
                                                                    new OamTeamUserWithUserId(
                                                                        termUserId,
                                                                        new OamTenantUserId(
                                                                            tenantUser.AggregateId),
                                                                        tenantUser.Payload.UserName)))
                                                            .Remap(_ => UnitValue.Unit)
                                                )
                                    )
                        )
                        .Remap(_ => true));
}
