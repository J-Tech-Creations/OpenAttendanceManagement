using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamUnconfirmedTenantUserInformation(
    OamTenantUserId TenantUserId,
    AuthIdentityId AuthIdentityId,
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamUnconfirmedTenantUserInformation Default => new(
        OamTenantUserId.Default,
        AuthIdentityId.Default,
        AuthIdentityEmail.Default);
}
