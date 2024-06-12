using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResultBoxes;
using System.Diagnostics;
namespace OpenAttendanceManagement.AuthCommon;

public class AuthDbContext(DbContextOptions<AuthDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    
}

public enum OamRoles
{
    SiteAdmin,
    User
}

public static class AppContextSeed
{
    // You can edit this with your own seed data
    public static ResultBox<UnitValue> Seed(AuthDbContext context)
        => ResultBox.Start
            .Scan(_ => CreateRoleIfNotExist(OamRoles.SiteAdmin.ToString(), context))
            .Scan(_ => CreateRoleIfNotExist(OamRoles.User.ToString(), context))
            .Scan(_ => CreateUserRoleFor("tomohisa@me.com", OamRoles.SiteAdmin.ToString(), context));
    
    public static ResultBox<UnitValue> CreateRoleIfNotExist(string name, AuthDbContext context)
        => ResultBox.Start
            .Conveyor(_ => context.Roles.Any(m => m.NormalizedName == name.ToUpper()) switch
            {
                true => new ApplicationException("Role already exists"),
                _ => ResultBox.Start
            })
            .ConveyorWrapTry(_ => context.Roles.Add(new IdentityRole { Name = name, NormalizedName = name.ToUpper() }))
            .Scan(_ => context.SaveChanges()).Remap(_ => UnitValue.Unit);
    
    public static ResultBox<UnitValue> CreateUserRoleFor(string email, string roleName, AuthDbContext context)
        => ResultBox.Start
            .Conveyor(
                _ => context.Users.FirstOrDefault(u => u.NormalizedEmail == email.ToUpper()) switch
                {
                    { } value => ResultBox.FromValue(value),
                    _ => new ApplicationException("User not found")
                })
            .Combine(
                _ => context.Roles.FirstOrDefault(u => u.NormalizedName == roleName.ToUpper())
                    switch
                {
                    { } value => ResultBox.FromValue(value),
                    _ => new ApplicationException("Role not found")
                })
            .Verify(values => context.UserRoles.Any(ur => ur.UserId == values.Value1.Id && ur.RoleId == values.Value2.Id) switch
            {
                true => new ApplicationException("User already in role"),
                _ => ExceptionOrNone.None
            })
            .Scan(
                (user, role) =>
                {
                    context.UserRoles.Add(
                        new IdentityUserRole<string>
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                })
            .Scan(_ => context.SaveChanges()).Remap(_ => UnitValue.Unit);
}