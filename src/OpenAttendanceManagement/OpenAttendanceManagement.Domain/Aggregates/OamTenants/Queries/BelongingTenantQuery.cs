using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;

public record BelongingTenantQuery(AuthIdentityEmail Email)
    : INextAggregateListQuery<OamTenant, BelongingTenantQuery.Record>
{
    public ResultBox<IEnumerable<Record>> HandleFilter(IEnumerable<AggregateState<OamTenant>> list,
        IQueryContext context)
        => ResultBox.FromValue(
            list.Where(m => m.Payload.Admins.Any(x => x.NormalizedEquals(Email)) ||
                            m.Payload.Users.Any(x => x.AuthIdentityEmail.NormalizedEquals(Email)))
                .Select(m => new Record(m.Payload.TenantCode.Value, m.Payload.TenantName.Value, m.AggregateId,
                    m.Payload.Users.FirstOrDefault(u => u.AuthIdentityEmail.NormalizedEquals(Email)) is { } user
                        ? user.GetType().Name
                        : nameof(OamUnconfirmedUncreatedTenantUserInformation))));

    public ResultBox<IEnumerable<Record>> HandleSort(IEnumerable<Record> filteredList, IQueryContext context) =>
        ResultBox.Ok(filteredList.OrderBy(m => m.TenantName).AsEnumerable());

    public record Record(string TenantCode, string TenantName, Guid TenantId, string UserClassName);
}