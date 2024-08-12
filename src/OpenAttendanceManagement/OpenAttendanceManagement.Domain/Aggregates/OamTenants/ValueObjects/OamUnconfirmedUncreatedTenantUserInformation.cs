using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamUnconfirmedUncreatedTenantUserInformation(
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamUnconfirmedUncreatedTenantUserInformation Default => new(
        AuthIdentityEmail.Default);
}