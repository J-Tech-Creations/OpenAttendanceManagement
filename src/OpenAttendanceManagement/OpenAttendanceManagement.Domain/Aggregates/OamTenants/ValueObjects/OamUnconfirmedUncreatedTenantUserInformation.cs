using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamUnconfirmedUncreatedTenantUserInformation(
    OamTenantUserId TenantUserId,
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamUnconfirmedUncreatedTenantUserInformation Default => new(
        OamTenantUserId.Default,
        AuthIdentityEmail.Default);
}
