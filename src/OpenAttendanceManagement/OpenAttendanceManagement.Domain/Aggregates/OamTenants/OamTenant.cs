using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using Sekiban.Core.Aggregate;
using System.Collections.Immutable;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants;

public record OamTenant(
    TenantCode TenantCode,
    TenantName TenantName,
    ImmutableList<IOamTenantUserInformation> Users,
    ImmutableList<AuthIdentityEmail> Admins,
    bool IsDeleted) : IDeletableAggregatePayload<OamTenant>
{
    public static OamTenant CreateInitialPayload(OamTenant? _) =>
        new(
            TenantCode.Default,
            TenantName.Default,
            ImmutableList<IOamTenantUserInformation>.Empty,
            ImmutableList<AuthIdentityEmail>.Empty,
            false);
}
