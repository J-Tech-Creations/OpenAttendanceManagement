using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using Sekiban.Core.Command;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;

public record CreateOamTenant(TenantCode TenantCode, TenantName TenantName) : ITenantCommandCommon
{
    public string TenantId => TenantCode.ToString();
}
