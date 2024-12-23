using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Common.Exceptions;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record OamTenantUserAcceptInvite(TenantCode TenantCode, OamTenantId OamTenantId)
    : ITenantCommandWithHandlerForExistingAggregateAsync<OamTenant, OamTenantUserAcceptInvite>
{
    public static Task<ResultBox<EventOrNone<OamTenant>>> HandleCommandAsync(
        OamTenantUserAcceptInvite command,
        ICommandContext<OamTenant> context) =>
        context
            .GetRequiredService<IOamUserManager>()
            .Conveyor(manager => manager.GetExecutingUserEmail())
            .Remap(AuthIdentityEmail.FromString)
            .Verify(
                userEmail => context
                    .GetState()
                    .Payload
                    .Users
                    .Any(
                        u => u.AuthIdentityEmail.NormalizedEquals(userEmail) &&
                            u is OamUnconfirmedTenantUserInformation { AuthIdentityId.HasValue: true })
                    ? ExceptionOrNone.None
                    : new TenantUserNotAddedToTenantYetException(userEmail.Value))
            .Conveyor(userEmail => context.AppendEvent(new OamTenantUserAcceptedToAddToTenant(userEmail)));


    public static Guid SpecifyAggregateId(OamTenantUserAcceptInvite command) => command.OamTenantId.Value;
    public string GetTenantId() => TenantCode.Value;
}
