using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Events;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermUsers.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Commands;

public record CreateOamTermTenant(TenantCode TenantCode, OamTerm Term)
    : ITenantCommandWithHandler<OamTermTenant, CreateOamTermTenant>
{
    public static Guid SpecifyAggregateId(CreateOamTermTenant command) => Guid.NewGuid();
    public static ResultBox<UnitValue> HandleCommand(
        CreateOamTermTenant command,
        ICommandContext<OamTermTenant> context) => context.AppendEvent(new OamTermTenantCreated(command.Term));
    public string GetTenantId() => TenantCode.Value;
}
public record AddOamTermUserToTermTenant(
    TenantCode TenantCode,
    OamTermTenantId TermTenantId,
    OamTeamUserWithUserId User)
    : ITenantCommandWithHandlerForExistingAggregateAsync<OamTermTenant, AddOamTermUserToTermTenant>
{

    public static Guid SpecifyAggregateId(AddOamTermUserToTermTenant command) => command.TermTenantId.Value;
    public static Task<ResultBox<UnitValue>> HandleCommandAsync(
        AddOamTermUserToTermTenant command,
        ICommandContext<OamTermTenant> context) => throw new NotImplementedException();
    public string GetTenantId() => TenantCode.Value;
}
