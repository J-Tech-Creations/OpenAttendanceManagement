using OpenAttendanceManagement.Domain.Aggregates.OamTenants;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Commands;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.Domain.Aggregates.OamTermTenants.ValueObjects;
using Sekiban.Testing.SingleProjections;
using System.Collections.Immutable;
namespace OpenAttendanceManagement.Domain.Test;

public class OamTenantSpecs : AggregateTest<OamTenant, OamDomainDependency>
{
    [Fact]
    public void CreateOamTenant()
    {
        GivenCommand(new CreateOamTenant(new TenantCode("tenant1"), new TenantName("Tenant 1")));
        WhenCommand(
            new OamTenantAddAuthIdentity(
                new OamTenantId(GetAggregateId()),
                new AuthIdentityEmail("johndoe@example.com"),
                new TenantCode("tenant1")));
        ThenPayloadIs(
            new OamTenant(
                new TenantCode("tenant1"),
                new TenantName("Tenant 1"),
                [],
                [new AuthIdentityEmail("johndoe@example.com")],
                ImmutableDictionary<OamTerm, OamTermTenantId>.Empty,
                false));
    }

    [Fact]
    public void CanNotAddTwoSameEmail()
    {
        GivenCommand(new CreateOamTenant(new TenantCode("tenant1"), new TenantName("Tenant 1")));
        GivenCommand(
            new OamTenantAddAuthIdentity(
                new OamTenantId(GetAggregateId()),
                new AuthIdentityEmail("johndoe@example.com"),
                new TenantCode("tenant1")));
        WhenCommand(
            new OamTenantAddAuthIdentity(
                new OamTenantId(GetAggregateId()),
                new AuthIdentityEmail("johndoe@example.com"),
                new TenantCode("tenant1")));
        ThenThrowsAnException();
    }
}
