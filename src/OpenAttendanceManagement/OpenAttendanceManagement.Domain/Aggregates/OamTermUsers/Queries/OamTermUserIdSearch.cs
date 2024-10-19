using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.Queries;

public record OamTermUserIdSearch(TenantCode TenantCode, OamTermTenantId TermTenantId, OamTenantUserId TenantUserId)
    : ITenantNextAggregateQuery<OamTermUser, OamTermUserIdSearch, OptionalValue<OamTermUserId>>
{
    public static ResultBox<OptionalValue<OamTermUserId>> HandleFilter(
        IEnumerable<AggregateState<OamTermUser>> list,
        OamTermUserIdSearch query,
        IQueryContext context) => list
        .ToList()
        .ToResultBox()
        .Remap(
            Aggregates =>
                Aggregates.Any(
                    m => m.Payload.TermTenantId == query.TermTenantId && m.Payload.TenantUserId == query.TenantUserId)
                    ? OptionalValue.FromValue(
                        new OamTermUserId(
                            Aggregates.First(
                                    m => m.Payload.TermTenantId == query.TermTenantId &&
                                        m.Payload.TenantUserId == query.TenantUserId)
                                .AggregateId))
                    : OptionalValue<OamTermUserId>.Empty);
    public string GetTenantId() => TenantCode.Value;
}
