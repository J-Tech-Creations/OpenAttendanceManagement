using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;

public record SimpleTenantQuery(string NameFilter, int? PageSize, int? PageNumber)
    : INextAggregateListQueryWithPaging<OamTenant, SimpleTenantQuery, SimpleTenantQuery.Record>
{
    public static ResultBox<IEnumerable<Record>> HandleFilter(
        IEnumerable<AggregateState<OamTenant>> list,
        SimpleTenantQuery query,
        IQueryContext context) =>
        ResultBox.FromValue(
            list
                .Where(
                    m => string.IsNullOrWhiteSpace(query.NameFilter) ||
                        m.Payload.TenantName.Value.Contains(query.NameFilter) ||
                        m.Payload.TenantCode.Value.Contains(query.NameFilter))
                .Select(
                    m => new Record(
                        m.AggregateId,
                        m.Payload.TenantCode.Value,
                        m.Payload.TenantName.Value,
                        m.Payload.Admins.Select(x => x.Value).ToList())));

    public static ResultBox<IEnumerable<Record>> HandleSort(
        IEnumerable<Record> filteredList,
        SimpleTenantQuery query,
        IQueryContext context)
        => ResultBox.FromValue(
            filteredList.OrderBy(m => m.TenantCode).ThenBy(m => m.TenantName).AsEnumerable());

    public record Record(
        Guid TenantId,
        string TenantCode,
        string TenantName,
        List<string> AdminEmails);
}
