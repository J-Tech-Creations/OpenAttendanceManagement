using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using Sekiban.Core.Events;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;

public record OamTenantCreated(TenantCode TenantCode, TenantName TenantName)
    : IEventPayload<OamTenant, OamTenantCreated>
{

    public static OamTenant OnEvent(OamTenant aggregatePayload, Event<OamTenantCreated> ev) =>
        aggregatePayload with
        {
            TenantCode = ev.Payload.TenantCode, TenantName = ev.Payload.TenantName
        };
}
public record OamTenantNameChanged(TenantName TenantName)
    : IEventPayload<OamTenant, OamTenantNameChanged>
{
    public static OamTenant OnEvent(OamTenant aggregatePayload, Event<OamTenantNameChanged> ev) =>
        aggregatePayload with
        {
            TenantName = ev.Payload.TenantName
        };
}
