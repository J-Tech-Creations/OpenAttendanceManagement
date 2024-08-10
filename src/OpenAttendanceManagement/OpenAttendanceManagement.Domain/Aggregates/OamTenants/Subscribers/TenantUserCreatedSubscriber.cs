using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.Commands;
using Sekiban.Core;
using Sekiban.Core.Events;
using Sekiban.Core.PubSub;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Subscribers;

public class TenantUserCreatedSubscriber(ISekibanExecutor executor)
    : IEventSubscriber<OamTenantUserAddedToTenant, TenantUserCreatedSubscriber>
{
    public Task HandleEventAsync(Event<OamTenantUserAddedToTenant> ev)
        => executor.ExecuteCommand(new CreateOamTenantUserByEvent(ev));
}