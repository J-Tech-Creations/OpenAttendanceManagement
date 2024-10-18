using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Queries;

public record OamTermTenantsListQuery(TenantCode TenantCode, OamTenantId TenantId)
    : ITenantNextAggregateListQuery<OamTermTenant, OamTermTenantsListQuery, OamTermTenantsListQuery.Record>
{

    public static ResultBox<IEnumerable<Record>> HandleFilter(
        IEnumerable<AggregateState<OamTermTenant>> list,
        OamTermTenantsListQuery query,
        IQueryContext context) =>
        list.Select(m => new Record(m.AggregateId, m.Payload.Term.Start, m.Payload.Term.End)).ToResultBox();
    public static ResultBox<IEnumerable<Record>> HandleSort(
        IEnumerable<Record> filteredList,
        OamTermTenantsListQuery query,
        IQueryContext context) => throw new NotImplementedException();
    public string GetTenantId() => TenantCode.Value;
    public record Record(Guid TermTenantId, DateOnly StartDate, DateOnly EndDate);
}
public static class ToResultBoxExtensions
{
    public static Task<ResultBox<T>> ToResultBox<T>(this Task<T> source) where T : notnull =>
        ResultBox.FromValue(source);
    public static ResultBox<T> ToResultBox<T>(this T source) where T : notnull => ResultBox.FromValue(source);
}
