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
