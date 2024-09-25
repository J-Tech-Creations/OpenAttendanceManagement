using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers;

// This is a tenant based user. Multiple user with same auth identity can exist in different tenants.
public record OamTenantUser(
    OamTenantId TenantId,
    OptionalValue<AuthIdentityId> AuthIdentityId,
    AuthIdentityEmail Email,
    OamUserName UserName,
    OamDisplayName DisplayName) : ITenantAggregatePayload<OamTenantUser>
{
    public static OamTenantUser CreateInitialPayload(OamTenantUser? _) => new(
        OamTenantId.Default,
        OptionalValue<AuthIdentityId>.Empty,
        AuthIdentityEmail.Default,
        OamUserName.Default,
        OamDisplayName.Default);
}
