using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenAttendanceManagement.ApiService;
using OpenAttendanceManagement.AuthCommon;
using OpenAttendanceManagement.Common;
using OpenAttendanceManagement.Domain;
using OpenAttendanceManagement.Domain.Aggregates.OamTenants.Queries;
using OpenAttendanceManagement.Domain.Aggregates.OamTenantUsers.ValueObjects;
using OpenAttendanceManagement.ServiceDefaults;
using ResultBoxes;
using Sekiban.Core;
using Sekiban.Core.Dependency;
using Sekiban.Infrastructure.Postgres;
using Sekiban.Web.Authorizations;
using Sekiban.Web.Dependency;
using Sekiban.Web.OpenApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddProblemDetails();
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
builder.Services.AddAuthentication();
builder.AddNpgsqlDbContext<AuthDbContext>("authdb");
builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.AddSekibanWithDependency<OamDomainDependency>();
builder.AddSekibanPostgresDbWithAzureBlobStorage();
builder.AddSekibanWebFromDomainDependency<OamDomainDependency>(
    definition => definition.AuthorizationDefinitions =
        new AuthorizeDefinitionCollectionWithUserManager<IdentityUser>());
builder.Services.AddSwaggerGen(options => options.ConfigureForSekibanWeb());

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IOamUserManager, OamUserManager>();

builder.Services.AddTransient<IOamAuthentication, OamAuthentication>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering",
    "Scorching"
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable.Range(1, 5)
                .Select(
                    index =>
                        new WeatherForecast(
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                .ToArray();
            return forecast;
        })
    .WithOpenApi()
    .RequireAuthorization()
    ;

app.MapGet(
        "/user/roles",
        async (
            UserManager<IdentityUser> userManager,
            HttpContext httpContext,
            AuthDbContext authDbContext) =>
        {
            var userIdClaim =
                httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Results.NotFound("User not found.");

            var user = await userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null) return Results.NotFound($"User with ID {userIdClaim.Value} not found.");

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

app.MapGet("/user/tenants", (ISekibanExecutor sekibanExecutor, IOamUserManager oamUserManager) =>
        oamUserManager.GetExecutingUserEmail().Remap(AuthIdentityEmail.FromString)
            .Conveyor(email => sekibanExecutor.ExecuteQuery(new BelongingTenantQuery(email)))
            .ToResults()
    ).WithName("GetUserTenants").WithOpenApi()
    .RequireAuthorization();
app.MapIdentityApi<IdentityUser>();
app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.Migrate();
    AppContextSeed.Seed(dbContext);
}

app.Run();