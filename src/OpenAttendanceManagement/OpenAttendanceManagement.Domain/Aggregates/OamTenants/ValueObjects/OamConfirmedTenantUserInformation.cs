using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
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
}
