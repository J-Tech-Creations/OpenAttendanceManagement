using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using ResultBoxes;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record AddTermToTenant(
    TenantCode TenantCode,
    DateOnly Month,
    OamTermTenantId TermTenantId,
    int ReferenceVersion)
    : ITenantCommandWithHandlerWithVersionValidationForExistingAggregateAsync<OamTenant, AddTermToTenant>
{

    public static Guid SpecifyAggregateId(AddTermToTenant command) => Guid.NewGuid();
    public static Task<ResultBox<UnitValue>> HandleCommandAsync(
        AddTermToTenant command,
        ICommandContext<OamTenant> context) => throw new NotImplementedException();
    public string GetTenantId() => TenantCode.Value;
}
