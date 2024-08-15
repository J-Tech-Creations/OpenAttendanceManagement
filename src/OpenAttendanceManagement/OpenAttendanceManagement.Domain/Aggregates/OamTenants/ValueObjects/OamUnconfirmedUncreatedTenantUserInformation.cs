using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record OamUnconfirmedUncreatedTenantUserInformation(
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamUnconfirmedUncreatedTenantUserInformation Default => new(
        AuthIdentityEmail.Default);

    public OptionalValue<OamTenantUserId> GetUserId() => OptionalValue<OamTenantUserId>.Empty;
}