using System.Reflection;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Subscribers;
using Sekiban.Core.Dependency;

namespace OpenAttendanceManagement.Domain;

public class OamDomainDependency : DomainDependencyDefinitionBase
{
    public override Assembly GetExecutingAssembly() => Assembly.GetExecutingAssembly();

    public override void Define()
    {
        AddAggregate<OamTenant>()
            .AddEventSubscriber<OamTenantUserAddedToTenant, TenantUserCreatedSubscriber>();
    }
}