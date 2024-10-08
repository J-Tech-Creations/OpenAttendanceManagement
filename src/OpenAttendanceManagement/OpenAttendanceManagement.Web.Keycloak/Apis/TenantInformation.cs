using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using ResultBoxes;
namespace OpenAttendanceManagement.Web.Keycloak.Apis;

public record TenantInformation(
    OptionalValue<BelongingTenantQuery.Record> Tenant,
    List<BelongingTenantQuery.Record> Tenants)
{
    public static TenantInformation Empty => new(
        OptionalValue<BelongingTenantQuery.Record>.Empty,
        new List<BelongingTenantQuery.Record>());

    public OptionalValue<BelongingTenantQuery.Record> Tenant { get; set; } = Tenant;
    public List<BelongingTenantQuery.Record> Tenants { get; set; } = Tenants;

    public ResultBox<UnitValue> ChangeTenant(Guid tenantId)
        => ResultBox.Start
            .Do(() => { Tenant = Tenants.FirstOrDefault(t => t.TenantId == tenantId) ?? Tenant; });

    public ResultBox<UnitValue> ClearTenant()
        =>
            ResultBox.Start.Do(
                () =>
                {
                    Tenant = OptionalValue<BelongingTenantQuery.Record>.Empty;
                    Tenants = new List<BelongingTenantQuery.Record>();
                });
}
