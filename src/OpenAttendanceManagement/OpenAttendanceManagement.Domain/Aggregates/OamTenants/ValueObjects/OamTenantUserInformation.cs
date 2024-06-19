using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using System.Text.Json.Serialization;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

[JsonDerivedType(
    typeof(OamUnconfirmedUncreatedTenantUserInformation),
    nameof(OamUnconfirmedUncreatedTenantUserInformation))]
[JsonDerivedType(
    typeof(OamUnconfirmedTenantUserInformation),
    nameof(OamUnconfirmedTenantUserInformation))]
[JsonDerivedType(
    typeof(OamConfirmedTenantUserInformation),
    nameof(OamConfirmedTenantUserInformation))]
[JsonDerivedType(
    typeof(OamInactiveTenantUserInformation),
    nameof(OamInactiveTenantUserInformation))]
public interface IOamTenantUserInformation;
public record OamUnconfirmedUncreatedTenantUserInformation(
    OamTenantUserId TenantUserId,
    AuthIdentityEmail AuthIdentityEmail) : IOamTenantUserInformation
{
    public static OamUnconfirmedUncreatedTenantUserInformation Default => new(
        OamTenantUserId.Default,
        AuthIdentityEmail.Default);
}
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
