using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Aggregate;
using Sekiban.Core.Query.QueryModel;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.Queries;

public record OamTenantUsersQuery(TenantCode TenantCode)
    : ITenantNextAggregateListQuery<OamTenantUser, OamTenantUsersQuery, OamTenantUsersQuery.Record>
{

    public string GetTenantId() => TenantCode.Value;
    public static ResultBox<IEnumerable<Record>> HandleFilter(
        IEnumerable<AggregateState<OamTenantUser>> list,
        OamTenantUsersQuery query,
        IQueryContext context)
        => ResultBox.FromValue(
            list.Select(
                u => new Record(
                    OamTenantUserId.FromAggregate(u),
                    u.Payload.UserName,
                    u.Payload.DisplayName,
                    u.Payload.Email,
                    u.Payload.AuthIdentityId.HasValue)));
    public static ResultBox<IEnumerable<Record>> HandleSort(
        IEnumerable<Record> filteredList,
        OamTenantUsersQuery query,
        IQueryContext context)
        => ResultBox.FromValue(filteredList.OrderBy(m => m.DisplayName.Value).AsEnumerable());

    public record Record(
        OamTenantUserId UserId,
        OamUserName UserName,
        OamDisplayName DisplayName,
        AuthIdentityEmail Email,
        bool IsUserCreated);
}
