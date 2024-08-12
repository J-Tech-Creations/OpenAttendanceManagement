using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record OamTenantCreateUser(
    OamTenantId OamTenantId,
    AuthIdentityEmail Email,
    TenantCode TenantCode,
    OamTenantUserId UserId,
    OamUserName UserName,
    OamDisplayName DisplayName)
    : ITenantCommandWithHandlerAsync<OamTenant, OamTenantCreateUser>
{
    public Guid GetAggregateId() => OamTenantId.Value;
    public string TenantId => TenantCode.Value;

    public static Task<ResultBox<UnitValue>> HandleCommandAsync(OamTenantCreateUser command,
        ICommandContext<OamTenant> context) =>
        context.GetRequiredService<IOamUserManager>()
            .Conveyor(manager => manager.GetExecutingUserEmail())
            .Remap(AuthIdentityEmail.FromString)
            .Verify(context.GetState().Payload.ValidateAdminUserEmail)
            .Verify(() => context.GetState().Payload.ValidateUserEmailShouldNotExists(command.Email))
            .Conveyor(() =>
                context.GetRequiredService<IOamUserManager>()
                    .Conveyor(manager => manager.GetUserIdFromEmail(command.Email.Value))
                    .Remap(AuthIdentityId.FromOptionalString)
                    .Conveyor(authIdentityId =>
                        context.AppendEvent(
                            new OamTenantUserAddedToTenant(command.Email, authIdentityId, command.UserId,
                                command.OamTenantId,
                                command.UserName,
                                command.DisplayName))));
}