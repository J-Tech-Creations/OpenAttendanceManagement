using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using Sekiban.Core.Aggregate;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermUsers;

public record OamTermUser(OamTermTenantId TermTenantId, OamTenantUserId TenantUserId, OamUserName UserName)
    : ITenantAggregatePayload<OamTermUser>
{
    public static OamTermUser CreateInitialPayload(OamTermUser? _) =>
        new(OamTermTenantId.Default, OamTenantUserId.Default, OamUserName.Default);
}
