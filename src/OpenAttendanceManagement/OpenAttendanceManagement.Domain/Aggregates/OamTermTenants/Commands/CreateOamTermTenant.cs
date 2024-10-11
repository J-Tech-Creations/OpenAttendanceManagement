using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.Commands;

public record CreateOamTermTenant(TenantCode TenantCode, OamTerm Term)
    : ITenantCommandWithHandler<OamTermTenant, CreateOamTermTenant>
{
    public static Guid SpecifyAggregateId(CreateOamTermTenant command) => Guid.NewGuid();
    public static ResultBox<UnitValue> HandleCommand(
        CreateOamTermTenant command,
        ICommandContext<OamTermTenant> context) => throw new NotImplementedException();
    public string GetTenantId() => TenantCode.Value;
}
