using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;

public record TenantFromCodeQuery(TenantCode TenantCode)
    : ITenantNextAggregateQuery<OamTenant, TenantFromCodeQuery, OptionalValue<AggregateState<OamTenant>>>
{

    public string GetTenantId() => TenantCode.Value;
    public static ResultBox<OptionalValue<AggregateState<OamTenant>>> HandleFilter(
        IEnumerable<AggregateState<OamTenant>> list,
        TenantFromCodeQuery query,
        IQueryContext context) => ResultBox.FromValue(OptionalValue.FromNullableValue(list.FirstOrDefault()));
}
