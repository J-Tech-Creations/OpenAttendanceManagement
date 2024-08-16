using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Query.QueryModel;

namespace OpenAttendanceManagement.Domain.Aggregates.Queries;

public record MyUserInformationQuery(TenantCode TenantCode)
    : ITenantNextGeneralQueryAsync<MyUserInformationQuery.Result>
{
    public Task<ResultBox<Result>> HandleFilterAsync(IQueryContext context) =>
        context.ExecuteQueryAsync(new TenantFromCodeQuery(TenantCode))
            .Conveyor(ResultBox.CheckOptionalEmpty)
            .Combine(() =>
                context.GetRequiredService<IOamUserManager>().Conveyor(manager => manager.GetExecutingUserEmail())
                    .Remap(AuthIdentityEmail.FromString))
            .Remap((tenant, email) =>
                OptionalValue.FromNullableValue(
                    tenant.Payload.Users.FirstOrDefault(m => m.AuthIdentityEmail.NormalizedEquals(email))))
            .Remap(optional =>
                optional.Match(value => value, () => OamUnconfirmedUncreatedTenantUserInformation.Default))
            .Conveyor(information => information.GetUserId().Match(
                userId => context.GetAggregateState<OamTenantUser>(userId.Value, TenantCode.Value)
                    .Conveyor(user => ResultBox.FromValue(new Result(OptionalValue.FromValue(
                        new ExistingUserResult(userId.Value, user.Payload.UserName,
                            user.Payload.DisplayName, information.AuthIdentityEmail)), information.GetType().Name))),
                ResultBox.FromValue(new Result(OptionalValue<ExistingUserResult>.Empty,
                    information.GetType().Name)))
            );

    public string GetTenantId() => TenantCode.Value;

    public record Result(
        OptionalValue<ExistingUserResult> ExistingUser,
        string CurrentUserState);

    public record ExistingUserResult(
        Guid UserId,
        OamUserName UserName,
        OamDisplayName DisplayName,
        AuthIdentityEmail Email);
}