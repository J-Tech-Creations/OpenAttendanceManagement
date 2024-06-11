using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace OpenAttendanceManagement.AuthCommon;

public class AuthDbContext(DbContextOptions<AuthDbContext> options)
    : IdentityDbContext<IdentityUser>(options);