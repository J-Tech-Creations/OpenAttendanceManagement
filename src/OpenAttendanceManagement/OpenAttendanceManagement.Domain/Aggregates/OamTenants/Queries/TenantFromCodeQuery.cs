using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;

public record TenantFromCodeQuery(TenantCode TenantCode)
    : ITenantNextAggregateQuery<OamTenant, OptionalValue<AggregateState<OamTenant>>>
{
    public ResultBox<OptionalValue<AggregateState<OamTenant>>> HandleFilter(IEnumerable<AggregateState<OamTenant>> list,
        IQueryContext context) => ResultBox.FromValue(OptionalValue.FromNullableValue(list.FirstOrDefault()));

    public string GetTenantId() => TenantCode.Value;
}