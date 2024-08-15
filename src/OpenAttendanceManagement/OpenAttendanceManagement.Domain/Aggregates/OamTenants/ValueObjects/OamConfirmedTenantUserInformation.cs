using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamConfirmedTenantUserInformation(
    OamTenantUserId TenantUserId,
    AuthIdentityId AuthIdentityId,
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamConfirmedTenantUserInformation Default => new(
        OamTenantUserId.Default,
        AuthIdentityId.Default,
        AuthIdentityEmail.Default);

    public OptionalValue<OamTenantUserId> GetUserId() => TenantUserId;
}