using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.ValueObjects;
using Sekiban.Core.Aggregate;
using System.Collections.Immutable;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants;

public record OamTermTenant(OamTerm Term, ImmutableArray<OamTeamUserWithUserId> TermUsers)
    : ITenantAggregatePayload<OamTermTenant>
{
    public static OamTermTenant CreateInitialPayload(OamTermTenant? _) =>
        new(OamTerm.Default, ImmutableArray<OamTeamUserWithUserId>.Empty);
}
