using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;

public record BelongingTenantQuery(AuthIdentityEmail Email)
    : INextAggregateListQuery<OamTenant, BelongingTenantQuery, BelongingTenantQuery.Record>
{
    public static ResultBox<IEnumerable<Record>> HandleFilter(
        IEnumerable<AggregateState<OamTenant>> list,
        BelongingTenantQuery query,
        IQueryContext context)
        => ResultBox.FromValue(
            list
                .Where(
                    m => m.Payload.Admins.Any(x => x.NormalizedEquals(query.Email)) ||
                        m.Payload.Users.Any(x => x.AuthIdentityEmail.NormalizedEquals(query.Email)))
                .Select(
                    m => new Record(
                        m.Payload.TenantCode.Value,
                        m.Payload.TenantName.Value,
                        m.AggregateId,
                        m.Payload.Users.FirstOrDefault(u => u.AuthIdentityEmail.NormalizedEquals(query.Email)) is
                            { } user
                            ? user.GetType().Name
                            : nameof(OamUnconfirmedUncreatedTenantUserInformation))));

    public static ResultBox<IEnumerable<Record>> HandleSort(
        IEnumerable<Record> filteredList,
        BelongingTenantQuery query,
        IQueryContext context) =>
        ResultBox.Ok(filteredList.OrderBy(m => m.TenantName).AsEnumerable());

    public record Record(string TenantCode, string TenantName, Guid TenantId, string UserClassName);
}
