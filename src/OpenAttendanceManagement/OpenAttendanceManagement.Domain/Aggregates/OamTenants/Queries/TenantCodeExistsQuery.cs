using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;

public record TenantCodeExistsQuery(TenantCode TenantCode) : INextAggregateQuery<OamTenant, TenantCodeExistsQuery, bool>
{
    public static ResultBox<bool> HandleFilter(
        IEnumerable<AggregateState<OamTenant>> list,
        TenantCodeExistsQuery query,
        IQueryContext context) =>
        ResultBox.WrapTry(() => list.Any(x => x.Payload.TenantCode.Value.Equals(query.TenantCode.Value)));
}
