using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamUnconfirmedUncreatedTenantUserInformation(
    AuthIdentityId AuthIdentityId,
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamUnconfirmedUncreatedTenantUserInformation Default => new(
        AuthIdentityId.Default,
        AuthIdentityEmail.Default);
}