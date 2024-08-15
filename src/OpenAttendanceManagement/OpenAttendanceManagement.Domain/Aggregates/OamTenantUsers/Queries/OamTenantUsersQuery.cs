using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.Queries;

public record OamTenantUsersQuery(TenantCode TenantCode)
    : ITenantNextAggregateListQuery<OamTenantUser, OamTenantUsersQuery.Record>
{
    public ResultBox<IEnumerable<Record>> HandleFilter(IEnumerable<AggregateState<OamTenantUser>> list,
        IQueryContext context) => ResultBox.FromValue(list.Select(u => new Record(OamTenantUserId.FromAggregate(u),
        u.Payload.UserName,
        u.Payload.DisplayName, u.Payload.Email, u.Payload.AuthIdentityId.HasValue)));

    public ResultBox<IEnumerable<Record>> HandleSort(IEnumerable<Record> filteredList, IQueryContext context) =>
        ResultBox.FromValue(filteredList.OrderBy(m => m.DisplayName.Value).AsEnumerable());

    public string TenantId => TenantCode.Value;

    public record Record(
        OamTenantUserId UserId,
        OamUserName UserName,
        OamDisplayName DisplayName,
        AuthIdentityEmail Email,
        bool IsUserCreated);
}