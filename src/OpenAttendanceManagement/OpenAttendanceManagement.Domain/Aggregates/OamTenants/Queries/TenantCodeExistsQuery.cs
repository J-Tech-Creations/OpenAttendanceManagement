using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;

public record TenantCodeExistsQuery(TenantCode TenantCode) : INextAggregateQuery<OamTenant, bool>
{
    public ResultBox<bool> HandleFilter(
        IEnumerable<AggregateState<OamTenant>> list,
        IQueryContext context) =>
        ResultBox.WrapTry(() => list.Any(x => x.Payload.TenantCode.Value.Equals(TenantCode.Value)));
}
