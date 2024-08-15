using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamUnconfirmedTenantUserInformation(
    OamTenantUserId TenantUserId,
    OptionalValue<AuthIdentityId> AuthIdentityId,
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamUnconfirmedTenantUserInformation Default => new(
        OamTenantUserId.Default,
        OptionalValue<AuthIdentityId>.Empty,
        AuthIdentityEmail.Default);

    public OptionalValue<OamTenantUserId> GetUserId() => TenantUserId;
}