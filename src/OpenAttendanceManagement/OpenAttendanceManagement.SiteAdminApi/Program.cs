using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.ServiceDefaults;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.AddSecurityDefinition(
            "bearerAuth",
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });
        c.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                            { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                    },
                    new string[] { }
                }
            });
    });

builder.Services.AddAuthorization();
builder.AddNpgsqlDbContext<AuthDbContext>("authdb");
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AuthDbContext>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapGet(
        "/user/roles",
        async (
            UserManager<IdentityUser> userManager,
            HttpContext httpContext,
            AuthDbContext authDbContext) =>
        {
            var userIdClaim =
                httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Results.NotFound("User not found.");
            }

            var user = await userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null)
            {
                return Results.NotFound($"User with ID {userIdClaim.Value} not found.");
            }

            var roles = authDbContext.UserRoles.Where(m => m.UserId == user.Id)
                .Select(m => m.RoleId)
                .Join(
                    authDbContext.Roles,
                    roleId => roleId,
                    role => role.Id,
                    (roleId, role) => role.Name)
                .Where(m => m != null)
                .ToList();
            return Results.Ok(roles);
        })
    .WithName("GetUserRoles")
    .WithOpenApi()
    .RequireAuthorization();

app.MapControllers();
app.MapIdentityApi<IdentityUser>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.Migrate();
    AppContextSeed.Seed(dbContext);
}

app.Run();
