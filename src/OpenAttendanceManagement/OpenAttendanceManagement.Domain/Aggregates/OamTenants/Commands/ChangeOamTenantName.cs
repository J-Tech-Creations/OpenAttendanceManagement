using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;

namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record ChangeOamTenantName(
    OamTenantId OamTenantId,
    TenantCode TenantCode,
    TenantName TenantName)
    : ITenantCommandWithHandlerAsync<OamTenant, ChangeOamTenantName>
{
    public string TenantId => TenantCode.Value;
    public Guid GetAggregateId() => OamTenantId.Value;

    public static Task<ResultBox<UnitValue>> HandleCommandAsync(
        ChangeOamTenantName command,
        ICommandContext<OamTenant> context) =>
        context.GetRequiredService<IOatAuthentication>()
            .Conveyor(oatAuth => oatAuth.GetOatLoginUser())
            .Verify(
                login => login.Roles.Contains(OamRoles.SiteAdmin.ToString())
                    ? ExceptionOrNone.None
                    : new ApplicationException("Not authorized"))
            .Conveyor(
                _ => context.AppendEvent(new OamTenantNameChanged(command.TenantName)).ToTask());
}