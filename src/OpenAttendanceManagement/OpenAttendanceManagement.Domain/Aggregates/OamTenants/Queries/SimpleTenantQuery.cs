using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;

public record SimpleTenantQuery(string NameFilter, int? PageSize, int? PageNumber)
    : INextAggregateListQueryWithPaging<OamTenant, SimpleTenantQuery.Record>
{
    public ResultBox<IEnumerable<Record>> HandleFilter(
        IEnumerable<AggregateState<OamTenant>> list,
        IQueryContext context) =>
        ResultBox.FromValue(
            list.Where(
                    m => string.IsNullOrWhiteSpace(NameFilter) ||
                        m.Payload.TenantName.Value.Contains(NameFilter) ||
                        m.Payload.TenantCode.Value.Contains(NameFilter))
                .Select(
                    m => new Record(
                        m.AggregateId.ToString(),
                        m.Payload.TenantCode.Value,
                        m.Payload.TenantName.Value)));

    public ResultBox<IEnumerable<Record>> HandleSort(
        IEnumerable<Record> filteredList,
        IQueryContext context) => ResultBox.FromValue(
        filteredList.OrderBy(m => m.TenantCode).ThenBy(m => m.TenantName).AsEnumerable());
    public record Record(string TenantId, string TenantCode, string TenantName);
}
