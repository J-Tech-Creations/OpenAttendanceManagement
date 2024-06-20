using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamInactiveTenantUserInformation(
    OamTenantUserId TenantUserId,
    AuthIdentityId AuthIdentityId,
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamInactiveTenantUserInformation Default => new(
        OamTenantUserId.Default,
        AuthIdentityId.Default,
        AuthIdentityEmail.Default);
}
