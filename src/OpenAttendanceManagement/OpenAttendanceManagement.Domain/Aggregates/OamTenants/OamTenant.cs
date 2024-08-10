using System.Collections.Immutable;
using OpenAttendanceManagement.Common.Exceptions;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants;

public record OamTenant(
    TenantCode TenantCode,
    TenantName TenantName,
    ImmutableList<IOamTenantUserInformation> Users,
    ImmutableList<AuthIdentityEmail> Admins,
    bool IsDeleted) : IDeletableAggregatePayload<OamTenant>
{
    public static OamTenant CreateInitialPayload(OamTenant? _) =>
        new(
            TenantCode.Default,
            TenantName.Default,
            ImmutableList<IOamTenantUserInformation>.Empty,
            ImmutableList<AuthIdentityEmail>.Empty,
            false);

    public ExceptionOrNone ValidateAdminUserEmail(AuthIdentityEmail email)
        =>
            Admins.Any(a => a.NormalizedEquals(email))
                ? ExceptionOrNone.None
                : new TenantUserDoesNotHaveAdminPrivilege(email.Value);

    public ExceptionOrNone ValidateUserEmailShouldNotExists(AuthIdentityEmail email)
        =>
            Users.All(a => !a.AuthIdentityEmail.NormalizedEquals(email))
                ? ExceptionOrNone.None
                : new TenantUserAlreadyExists("指定されたユーザーは既に存在します。" + email.Value);
}