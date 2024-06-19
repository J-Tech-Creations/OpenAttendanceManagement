using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using Sekiban.Core.Aggregate;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers;

// This is a tenant based user. Multiple user with same auth identity can exist in different tenants.
public record OamTenantUser(
    OamTenantId TenantId,
    AuthIdentityId AuthIdentityId,
    AuthIdentityEmail Email,
    OamUserName UserName,
    OamDisplayName DisplayName) : IAggregatePayload<OamTenantUser>
{
    public static OamTenantUser CreateInitialPayload(OamTenantUser? _) => new(
        OamTenantId.Default,
        AuthIdentityId.Default,
        AuthIdentityEmail.Default,
        OamUserName.Default,
        OamDisplayName.Default);
}
