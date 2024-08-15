using Sekiban.Core.Aggregate;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;

public record OamTenantUserId(Guid Value)
{
    public static OamTenantUserId Default => new(Guid.Empty);

    public static OamTenantUserId FromAggregate(AggregateState<OamTenantUser> user) => new(user.AggregateId);
}